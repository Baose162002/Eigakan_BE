using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class SubscriptionPackage
	{
		public string Id { get; set; }
		public string? PackageName { get; set; }
		public decimal? Price { get; set; }
		public int? Duration { get; set; }
		public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        public ICollection<SubscriptionPurchase>? SubscriptionPurchases { get; set; }
	}
}
