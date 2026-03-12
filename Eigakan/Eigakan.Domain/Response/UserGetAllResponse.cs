using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response
{
	public class UserGetAllResponse
	{
		public string Id { get; set; }
		public string FullName { get; set; }
		public bool Gender { get; set; }
		public string Birthday { get; set; }
		public string Picture { get; set; }
		[EmailAddress,Required]
		public string Email { get; set; }
		public DateTime CreateDate { get; set; }
		public string? Status { get; set; }

		public string RoleName { get; set; }
		public UserRegister? UserRegister { get; set; }
	}
}
