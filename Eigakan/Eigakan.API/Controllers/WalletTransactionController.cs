using Eigakan.Application.Interface;
using Eigakan.Domain.Request.AdPurchaseTransaction;
using Eigakan.Domain.Request.WalletTranasction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IWalletTransactionService _walletTransactionService;
        
        public WalletTransactionController(IVnPayService vnPayService, IWalletTransactionService walletTransactionService)
        {
            _vnPayService = vnPayService;
            _walletTransactionService = walletTransactionService;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] WalletTransactionCreateRequest value)
        {
            if (value == null)
            {
                return BadRequest(new { success = false, message = "No orders provided" });
            }

            var results = await _walletTransactionService.CreatePayment(value);

            if (results.Success != false)
            {
                return Ok(new
                {
                    results.Success,
                    results.Message,
                    results.Data
                });
            }
            return BadRequest(new
            {
                results.Success,
                results.Message
            });
        }

        [HttpGet("payment_return")]
        public async Task<IActionResult> getPaymentReturn()
        {
            var paymentResult = _vnPayService.ValidatePayment(Request.Query);



            var vnp_TransactionNo = Request.Query["vnp_TransactionNo"].ToString();
            var orderInfo = Request.Query["vnp_OrderInfo"].ToString();
            var transactionStatus = Request.Query["vnp_TransactionStatus"].ToString();

            if (string.IsNullOrEmpty(orderInfo) || orderInfo.Length < 36)
            {
                return BadRequest(new { success = false, message = "Invalid order information" });
            }


            WalletTransactionStatus status = new WalletTransactionStatus
            {
                vnp_TransactionNo = vnp_TransactionNo,
                WalletTransactionID = orderInfo,
                Status = transactionStatus

            };
            var results = await _walletTransactionService.PaymentReturn(status);

            if (results.Success != false)
            {
                return Ok(new
                {
                    results.Success,
                    results.Message,
                    results.Data
                });
            }
            return BadRequest(new
            {
                results.Success,
                results.Message,

            });
        }

        [HttpGet("MyHistoryWallet")]
        [Authorize]
        public async Task<IActionResult> GetMyTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _walletTransactionService.GetListTransactionForCurrentUser(page, pageSize);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
