using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.UserRequest
{
	public class UserCreateRequest
	{
		public string? FullName { get; set; }
		[EmailAddress]
		public string? Email { get; set; }
		public string? RoleId { get; set; }
		public string? UserRegisterId { get; set; }
	}
}
