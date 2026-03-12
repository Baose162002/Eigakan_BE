using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.ContractResponse
{
	public class ContractGetName
	{
		public string Id { get; set; }
		public DateTime? StartDate { get; set; }
		public string? DistributorName { get; set; }
		public string? Status { get; set; }
	}
}
