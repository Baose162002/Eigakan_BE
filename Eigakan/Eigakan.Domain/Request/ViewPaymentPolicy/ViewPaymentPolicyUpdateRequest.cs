using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.ViewPaymentPolicy
{
	public class ViewPaymentPolicyUpdateRequest
	{
		public decimal PricePerView { get; set; }
		public decimal WebSharePercentage { get; set; }
	}
}
