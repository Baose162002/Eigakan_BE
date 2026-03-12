using Eigakan.Domain.Request.PayOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IPayOSService
    {
        Task<string> CreatePaymentUrl(PayOSRequest request);
     
    }
}
