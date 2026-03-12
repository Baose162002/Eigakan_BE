using Eigakan.Application.Interface;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.SubscriptionPackageResponse;
using Eigakan.Domain.Response.VNPayResponse;
using Eigakan.Domain.Request.VNPayRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Eigakan.Application.Helper
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public string CreatePaymentUrl(VnPayRequest request)
        {
            var vnp_TmnCode = _configuration["VNPAY:TmnCode"];
            var vnp_HashSecret = _configuration["VNPAY:HashSecret"];
            var vnp_Url = _configuration["VNPAY:Url"];
          
            var transactionRef = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var amountInVND = ((long)(request.Amount * 100)).ToString();
            var timeNow = DateTime.UtcNow.AddHours(7); // Chuyển sang giờ Việt Nam
            var expireDate = timeNow.AddMinutes(2); 
            
            // Dictionary chứa các tham số VNPAY
            var vnp_Params = new Dictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", amountInVND },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", transactionRef },
                { "vnp_OrderInfo", request.OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", request.ReturnUrl },
                { "vnp_IpAddr", request.IpAddress },
                { "vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss") }
            };

            // Sắp xếp tham số theo thứ tự từ điển
            var sortedParams = vnp_Params.OrderBy(p => p.Key);
            
            // Tạo query string
            var queryString = string.Join("&", sortedParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            // Mã hóa HMAC SHA512
            var secureHash = HmacSHA512(vnp_HashSecret, queryString);
         
            // Tạo URL hoàn chỉnh
            return $"{vnp_Url}?{queryString}&vnp_SecureHash={secureHash}";
        }





        public VnPayResponse ProcessReturn(Dictionary<string, string> vnp_Params)
        {
            var vnp_HashSecret = _configuration["VnPay:HashSecret"];
            string vnp_SecureHash = vnp_Params["vnp_SecureHash"];
            vnp_Params.Remove("vnp_SecureHash");

            string rawData = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            string calculatedHash = HmacSHA512(vnp_HashSecret, rawData);

            if (calculatedHash == vnp_SecureHash)
            {
                return new VnPayResponse
                {
                    Success = vnp_Params.ContainsKey("vnp_ResponseCode") && vnp_Params["vnp_ResponseCode"] == "00",
                    OrderId = vnp_Params.ContainsKey("vnp_TxnRef") ? vnp_Params["vnp_TxnRef"] : "Unknown",
                    Amount = vnp_Params.ContainsKey("vnp_Amount") ? Convert.ToDecimal(vnp_Params["vnp_Amount"]) / 100 : 0,
                    Message = vnp_Params.ContainsKey("vnp_Message") ? vnp_Params["vnp_Message"] : "No message"
                };
            }

            return new VnPayResponse { Success = false, Message = "Invalid signature" };
        }

        public string GetReturnUrl(string type)
        {
            return _configuration[$"VnPay:ReturnUrls:{type}"] ?? _configuration["VnPay:DefaultReturnUrl"];
        }
        public VnPayResponse ValidatePayment(IQueryCollection queryParams)
        {
            var vnp_Params = queryParams.ToDictionary(k => k.Key, v => v.Value.ToString());
            return ProcessReturn(vnp_Params);
        }


        private static string HmacSHA512(string key, string data)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return string.Concat(hashValue.Select(b => b.ToString("x2"))).ToLower();
            }

        }




    }
}
