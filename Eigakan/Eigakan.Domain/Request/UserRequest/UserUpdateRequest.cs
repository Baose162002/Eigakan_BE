using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.UserRequest
{
	public class UserUpdateRequest
	{
		public string? FullName { get; set; }
		public bool? Gender { get; set; }
		public string? Birthday { get; set; }
		public string? Picture { get; set; }
	}
}
