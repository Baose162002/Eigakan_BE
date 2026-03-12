using Eigakan.Domain.Request.AdPurchaseSlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdPurchaseTransaction
{
    public class AdPurchaseTransactionCreateRequest
    {
        public List<AdPurchaseSlotCreateRequest> orders { get; set; }

       
    }
}
