using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.AdPurchaseItem
{
    public class AdPurchaseItemGetAllResponse
    {
        public string? Id { get; set; }
        public int? ViewQuantity { get; set; }
        public decimal? PricePerView { get; set; }
        public decimal? ConsumedViewFee { get; set; }
        public decimal? RefundedPrice { get; set; }
        public decimal? Price { get; set; }
        public int? RemainingViews { get; set; }
        public string? Status { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? AdPackageId { get; set; }
        public string? AdPackageName { get; set; }

        public string? AdMediaId { get; set; }
        public string? AdMediaUrl { get; set; }
        public string? AdMediaStatus { get; set; }

        public string? AdPurchaseTransactionId { get; set; }
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
    }

}
