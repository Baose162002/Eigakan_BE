using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.MovieEarning
{
	public class MovieEarningResponse
	{
		public string? Id { get; set; }
		public DateOnly? StartWeek { get; set; }
		public DateOnly? EndWeek { get; set; }
		public int? TotalView { get; set; }
		public decimal? TotalEarnings { get; set; }
		public bool? Status { get; set; }
		public DateTime? CreateDate { get; set; }
		public string? UserId { get; set; }
		public string? MovieId { get; set; }
		public string? MovieName { get; set; }
	}
}
