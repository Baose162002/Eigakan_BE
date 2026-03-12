using Eigakan.Domain.Models;
using Eigakan.Domain.Request.VNPayRequest;
using Eigakan.Domain.Response.VNPayResponse;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPayRequest request);
        VnPayResponse ProcessReturn(Dictionary<string, string> vnp_Params);
        string GetReturnUrl(string type);
        VnPayResponse ValidatePayment(IQueryCollection queryParams);
    }



}
