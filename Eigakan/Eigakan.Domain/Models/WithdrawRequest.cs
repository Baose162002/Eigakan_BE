using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class WithdrawRequest
	{
		public string? Id { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? RequestAt { get; set; }
        public string? RequestBy { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessedBy { get; set; }
        public string? Status { get; set; }
        public string? UserEarningId { get; set; }
        public UserEarning? UserEarning { get; set; }
    }
}
