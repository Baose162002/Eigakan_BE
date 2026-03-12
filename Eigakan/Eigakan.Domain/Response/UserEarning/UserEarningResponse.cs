using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.UserEarning
{
	public class UserEarningResponse
	{
		public string? Id { get; set; }
		public DateOnly? StartWeek { get; set; }
		public DateOnly? EndWeek { get; set; }
		public int? TotalView { get; set; }
		public decimal? TotalEarnings { get; set; }
		public decimal? WebEarnings { get; set; }
		public decimal? FinalEarnings { get; set; }
		public bool? PaymentStatus { get; set; }
		public DateTime? CreateDate { get; set; }
		public string? UserId { get; set; }
		public string? UserName { get; set; }
	}
}
