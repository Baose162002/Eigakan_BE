using Eigakan.Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IEmailService
	{
		Task SendEmailAsync(EmailSetting.MailResponse mailrequest);
	}
}
