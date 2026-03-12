using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.UserRegisterRequest
{
	public class UserRegisterCreateRequest
	{
		public string? FullName { get; set; }
		[Required, EmailAddress]
		public string? Email { get; set; }
		[RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
		public string? PhoneNumber { get; set; }
		public string? Reason { get; set; }
		public string? FileUrl { get; set; }
	}
}
