using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class ViewPaymentPolicy
	{
        public string? Id { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public decimal PricePerView { get; set; }
        public decimal WebSharePercentage { get; set; }
        public string?  Status { get; set; }
    }
}
