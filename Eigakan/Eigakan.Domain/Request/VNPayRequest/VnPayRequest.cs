using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.VNPayRequest
{
    public class VnPayRequest
    {
        public string OrderId { get; set; }       
        public decimal Amount { get; set; }       
        public string OrderInfo { get; set; }    
        public string IpAddress { get; set; }
        public string ReturnUrl { get; set; }
    }

}
