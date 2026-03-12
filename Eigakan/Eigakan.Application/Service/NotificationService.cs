using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class NotificationService
	{
		private readonly Webhook _webhook;

		public NotificationService(Webhook webhook)
		{
			_webhook = webhook;
		}

		public async Task SendNotificationAsync()
		{
			// Gửi thông báo đến Discord với nội dung
			var response = await _webhook.Send("tao bị khùng");

			// Kiểm tra phản hồi và ghi nhật ký
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Thông báo đã được gửi thành công.");
			}
			else
			{
				Console.WriteLine("Có lỗi khi gửi thông báo.");
			}
		}
	}
}