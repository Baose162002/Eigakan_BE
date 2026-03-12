using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdPurchaseSlot
{
    public class AdPurchaseSlotCreateRequest
    {

        public string? AdSlotTimeID { get; set; }        
        public string? AdPackageID { get; set; }

    }
}
