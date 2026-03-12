using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Helper
{
	public class EmailSetting
	{
		public class MailResponse
		{
			public string ToEmail { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
		}
		public class EmailSettings
		{
			public string Email { get; set; }
			public string Password { get; set; }
			public string Host { get; set; }
			public string Displayname { get; set; }
			public int Port { get; set; }
		}
	}
}
