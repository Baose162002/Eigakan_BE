using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.MovieEarning
{
	public class MovieEarningDashboardResponse
	{
		public List<MovieEarningResponse> MovieEarning { get; set; } = new();
		public int Total{ get; set; }
		public int TotalView { get; set; }
		public decimal TotalEarnings { get; set; }
		public decimal TotalEarningsMovieContract { get; set; }
	}
}
