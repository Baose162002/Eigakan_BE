using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class AdPurchaseItems
	{
        public string? Id { get; set; }
        public int? ViewQuantity { get; set; }
        public decimal? PricePerView { get; set; }
        public decimal? Price { get; set; }
        public decimal? ConsumedViewFee { get; set; }
		public decimal? RefundedPrice { get; set; }
        public int? RemainingViews { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; } 

        public string? AdPackageId { get; set; }
        public AdPackage? AdPackage { get; set; }

        public string? AdMediaId { get; set; }
        public AdMedia? AdMedia { get; set; }

        public string? AdPurchaseTransactionId { get; set; }
        public AdPurchaseTransaction? AdPurchaseTransaction { get; set; }
    }
}
