using Eigakan.Application.Interface.IRepository;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using static Eigakan.Application.Helper.EmailSetting;
using MailKit.Net.Smtp;

namespace Eigakan.Application.Service
{
	public class EmailService : IEmailService, IDisposable
	{
		private readonly EmailSettings _emailSettings;
		private readonly SmtpClient _smtpClient;

		public EmailService(IOptions<EmailSettings> options)
		{
			_emailSettings = options.Value;
			_smtpClient = new SmtpClient();
			
		}

		private async Task InitializeSmtpClientAsync()
		{
			if (!_smtpClient.IsConnected)
			{
				await _smtpClient.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
				await _smtpClient.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
			}
		}

		public async Task SendEmailAsync(MailResponse mailrequest)
		{
			await InitializeSmtpClientAsync();

			var email = new MimeMessage();
			email.Sender = MailboxAddress.Parse(_emailSettings.Email);
			email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
			email.Subject = mailrequest.Subject;

			var builder = new BodyBuilder
			{
				HtmlBody = mailrequest.Body
			};
			email.Body = builder.ToMessageBody();

			try
			{
				await _smtpClient.SendAsync(email);
			}
			catch (Exception ex)
			{
				// Ghi log lỗi
				// _logger.LogError(ex, "Error sending email"); // Đảm bảo có logger để ghi lại lỗi
				throw; // Ném lại ngoại lệ nếu cần
			}
		}

		public void Dispose()
		{
			if (_smtpClient.IsConnected)
			{
				_smtpClient.Disconnect(true);
			}
			_smtpClient.Dispose();
		}
	}
}
