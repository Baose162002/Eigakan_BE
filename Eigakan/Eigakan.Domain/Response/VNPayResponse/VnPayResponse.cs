using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.VNPayResponse
{
    public class VnPayResponse
    {
        public bool Success { get; set; }       
        public string OrderId { get; set; }     
        public decimal Amount { get; set; }     
        public string Message { get; set; }    
    }

}
