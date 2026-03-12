using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.VNPayRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;
using System.Security.Claims;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPurchasePaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ISubscriptionPackageService _subscriptionPackageService;
        private readonly ISubscriptionPurchaseService _subscriptionPurchaseService;

        public SubscriptionPurchasePaymentController(
            IVnPayService vnPayService,
            ISubscriptionPackageService subscriptionPackageService,
            ISubscriptionPurchaseService subscriptionPurchaseService)
        {
            _vnPayService = vnPayService;
            _subscriptionPackageService = subscriptionPackageService;
            _subscriptionPurchaseService = subscriptionPurchaseService;
        }
        
        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return "Invalid IP:" + ex.Message;
            }

            return "127.0.0.1";
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var paymentResult = _vnPayService.ValidatePayment(Request.Query);

            if (!paymentResult.Success)
            {
                return BadRequest(new { success = false, message = "Payment failed or invalid signature" });
            }

            var orderInfo = Request.Query["vnp_OrderInfo"].ToString();
            string vnpTxnRef = Request.Query["vnp_TxnRef"].ToString();
            if (string.IsNullOrEmpty(orderInfo) || orderInfo.Length < 36)
            {
                return BadRequest(new { success = false, message = "Invalid order information" });
            }

            string subscriptionId = orderInfo.Substring(13, 24);
            string userId = orderInfo.Substring(38);

            var subscriptionPackage = await _subscriptionPackageService.GetSubscriptionPackageById(subscriptionId);
            if (!subscriptionPackage.Success || subscriptionPackage.Data == null)
            {
                return BadRequest(new { success = false, message = "Invalid Subscription Package" });
            }

            int durationInDays = (int)subscriptionPackage.Data.Duration;

            var latestSubscription = await _subscriptionPurchaseService.GetLatestUserSubscription(userId);
            if (latestSubscription != null && latestSubscription.ExpiredDate > DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "You already have an active subscription. Please wait until it expires before purchasing a new one." });
            }

           
            DateTime vietnamTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                           TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            DateTime newExpiredDate = vietnamTimeNow.AddDays(durationInDays);

            var subscriptionPurchase = new SubscriptionPurchase
            {
                Id = Guid.NewGuid().ToString(),
                PurchaseDate = vietnamTimeNow, 
                ExpiredDate = newExpiredDate, 
                TotalPrice = paymentResult.Amount,
                Status = "Active",
                PaymentMethod = "VNPay",
                PaymentReferenceID = vnpTxnRef,
                SubscriptionId = subscriptionId,
                UserId = userId
            };

            await _subscriptionPurchaseService.SavePurchaseAsync(subscriptionPurchase);
            await _subscriptionPurchaseService.UpdateStatusUserSubscriptionPurchase(userId);

            return Ok(new { success = true, message = "Payment successful", data = subscriptionPurchase });
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetAllSubscriptionPurchase([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
          [FromQuery] string? id = null, [FromQuery] DateTime? startDate=null, [FromQuery] DateTime? endDate=null, [FromQuery] DateTime? expiredDate = null,
        [FromQuery] decimal? totalPrice = null, [FromQuery] string? status = null, [FromQuery] string? subscriptionId = null, [FromQuery] string? userId = null)
        {
            var result = await _subscriptionPurchaseService.GetAllSubscriptionPurchaseAsync(page, pageSize, id, startDate, endDate, expiredDate, totalPrice, status, subscriptionId, userId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(new
            {
                Success = result.Success,
                Message = result.Message,
                Data = new
                {
                    Total = result.Data.Total,
                    TotalEarnings = result.Data.totalEarnings,
                    ActiveSubscriptionCount = result.Data.ActiveSubscriptionCount,
                    SubscriptionPurchase = result.Data.SubscriptionPurchases,
                }
            });
        }
        
        [HttpGet("GetAllSubscriptionPurchaseUser")]
		[Authorize(Roles = "MEMBER,VIP MEMBER")]
		public async Task<IActionResult> GetAllSubscriptionPurchaseUser([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(MySetting.CLAIM_USERID)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated." });
            }

            var result = await _subscriptionPurchaseService.GetAllSubscriptionPurchaseUser(userId, page, pageSize);

            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    Total = result.Data.Total,
                    SubscriptionPurchase = result.Data.SubscriptionPurchases
                }
            });
        }

		[Authorize(Roles = "MEMBER")]
		[HttpPost("create")]
		public async Task<IActionResult> CreatePayment([FromQuery] string subscriptionId)
		{
			var userId = User.FindFirst(MySetting.CLAIM_USERID)?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new { success = false, message = "User not authenticated" });

			var subscription = await _subscriptionPackageService.GetSubscriptionPackageById(subscriptionId);
			if (!subscription.Success || subscription.Data == null)
				return BadRequest(new { success = false, message = "Invalid Subscription Package" });

			var vnPayRequest = new VnPayRequest
			{
				OrderId = subscriptionId,
				Amount = (decimal)subscription.Data.Price,
				OrderInfo = $"Subscription-{subscription.Data.Id}-{userId}",
				ReturnUrl = _vnPayService.GetReturnUrl("SubscriptionPurchase"),
				IpAddress = GetIpAddress(HttpContext)
			};

			string paymentUrl = _vnPayService.CreatePaymentUrl(vnPayRequest);
			return Ok(new { success = true, paymentUrl });
		}


	}
}
