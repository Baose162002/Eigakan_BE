using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class AdPurchaseTransaction
	{
		public string? Id { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        [JsonIgnore]
		public User? User { get; set; }

        public ICollection<AdPurchaseItems>? AdPurchaseItems { get; set; }

    }
}
