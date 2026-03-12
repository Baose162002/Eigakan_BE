using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AuthRequest
{
	public class RegisterRequest
	{
		[Required, EmailAddress]
		public string Email { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
		[Required, Compare("Password")]
		public string ConfirmPassword { get; set; } = string.Empty;
		[Required]
		[RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Full name can only contain letters and spacssses.")]
		public string? FullName { get; set; }

	}
}
