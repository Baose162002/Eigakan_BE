using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.ViewPaymentPolicy
{
	public class ViewPaymentPolicyCreateRequest
	{
		public DateOnly? EffectiveDate { get; set; }
		public decimal PricePerView { get; set; }
		public decimal WebSharePercentage { get; set; }
	}
}
