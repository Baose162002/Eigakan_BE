using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.SubscriptionPurchaseRequest
{
    public class SubscriptionPurchaseCreeateRequest
    {
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }

        public string? SubscriptionId { get; set; }
        public string? UserId { get; set; }
    }
}
