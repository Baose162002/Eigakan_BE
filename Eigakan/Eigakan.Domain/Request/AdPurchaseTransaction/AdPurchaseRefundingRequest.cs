using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdPurchaseTransaction
{
    public class AdPurchaseRefundingRequest
    {
        public string? BankName { get; set; }
        public string? BankNumber  { get; set; }
    }
}
