using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class UserRegister
	{
		public string Id { get; set; }
		[MaxLength(255)]
		public string? FullName { get; set; }
		[MaxLength(100)]
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		[MaxLength(500)]
		public string? Reason { get; set; }
		[MaxLength(1000)]
		public string? ReasonForRejection { get; set; }
		[MaxLength(1000)]
		public string? FileUrl { get; set; }
		public DateTime CreateDate {  get; set; }
		[MaxLength(100)]
		public string? Status { get; set; }
		[JsonIgnore]
		public User? User { get; set; }
	}
}
