using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdPurchase
{
    public class AdPurchaseCreateRequest
    {


     
        public decimal TotalPrice { get; set; }

        public string UserID { get; set; }
    }
}
