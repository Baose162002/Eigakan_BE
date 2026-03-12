using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.PayOS
{
    public class PayOSRequest
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public List<PayOSItem> Items { get; set; } // Sử dụng List<T> ở đây
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
