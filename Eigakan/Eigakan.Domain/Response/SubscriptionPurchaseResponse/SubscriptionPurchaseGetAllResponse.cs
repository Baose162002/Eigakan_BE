using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.SubscriptionPurchaseResponse
{
    public class SubscriptionPurchaseGetAllResponse
    {
        public string Id { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }
        public string? SubscriptionId { get; set; }
		public string? PaymentMethod { get; set; }
		public string? PaymentReferenceID { get; set; }
		public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
