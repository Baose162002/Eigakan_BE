using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.AdPurchaseTransaction
{
    public class AdPurchaseTransactionGetAllResponse
    {
        public string? Id { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }

		public ICollection<AdPurchaseItemsResponse>? AdPurchaseItems { get; set; }
	}

	public class AdPurchaseItemsResponse
	{
		public string? Id { get; set; }
		public int? ViewQuantity { get; set; }
		public decimal? PricePerView { get; set; }
		public decimal? Price { get; set; }
		public string? Status { get; set; }
	}
}

