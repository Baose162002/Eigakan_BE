using Eigakan.Application.Interface;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.PayOS;
using Eigakan.Domain.Request.VNPayRequest;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Helper
{
    public class PayOSService :IPayOSService
    {
        private readonly IConfiguration _configuration;
       
        public PayOSService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreatePaymentUrl(PayOSRequest request)
        {
            try
            {
                var clientId = _configuration["PAYOS:CLIENT_ID"];
                var apiKey = _configuration["PAYOS:API_KEY"];
                var checksumKey = _configuration["PAYOS:CHECKSUM_KEY"];

                var payOS = new PayOS(clientId, apiKey, checksumKey);
                var paymentLinkRequest = new PaymentData(
                orderCode: int.Parse(DateTimeOffset.UtcNow.ToString("ffffff")),
                amount: request.Amount,
                description: request.Description,
                items: request.Items.Select(i => new ItemData(i.Name, i.Quantity, i.Price)).ToList(),
                returnUrl: request.ReturnUrl,
                cancelUrl: request.CancelUrl
            );
                var response = await payOS.createPaymentLink(paymentLinkRequest);
                return response.checkoutUrl;
            }
            catch (Exception ex)
            {
                return $"Failed to payment: {ex.Message}";
            }
        }

        public static string ComputeHmacSha256(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

     

        

    }
}
