using Eigakan.Domain.Response.MovieEarning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.UserEarning
{
	public class UserEarningDashboardResponse
	{
		public List<UserEarningResponse> userEarnings { get; set; } = new();
		public int Total { get; set; }
		public decimal TotalEarnings { get; set; }
		public decimal WebEarnings { get; set; }
		public decimal FinalEarnings { get; set; }
	}
}
