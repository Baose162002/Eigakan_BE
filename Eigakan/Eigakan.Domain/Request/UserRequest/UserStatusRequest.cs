using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.UserRequest
{
	public class UserStatusRequest
	{
		public string? Id { get; set; }
		public int Status { get; set; }
	}
}
