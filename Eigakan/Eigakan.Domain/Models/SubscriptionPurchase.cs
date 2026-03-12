using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class SubscriptionPurchase
	{
		public string Id { get; set; }
		public DateTime? PurchaseDate { get; set; }
		public DateTime? ExpiredDate { get; set; }
		public decimal? TotalPrice { get; set; }
		public string? Status { get; set; }
		public string? PaymentMethod { get; set; }
		public string? PaymentReferenceID { get; set; }
		public string? SubscriptionId { get; set; }
		public SubscriptionPackage? SubscriptionPackage { get; set; }

		public string? UserId { get; set; }
		public User? User { get; set; }
	}
}
