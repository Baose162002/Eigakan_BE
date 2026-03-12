using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eigakan.Application.Helper.Logging
{
	public class Logger
	{
		private readonly Webhook _webhook;

		public Logger(Webhook webhook)
		{
			_webhook = webhook;
		}

		public virtual async Task LogError(Exception ex, string controllerName)
		{
			string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string message = $"[{currentDateTime}] Error occurred in {controllerName}: {ex.Message}";

			// Gửi log lỗi vào Discord webhook
			await _webhook.Send(message);
		}

		public virtual async Task LogAnnoucement(object data, string controllerName)
		{
			string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

			// Chuyển object thành JSON
			string jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

			string message = $"[{currentDateTime}] Announcement in {controllerName}:\n```json\n{jsonData}\n```";

			// Gửi log lên Discord webhook
			await _webhook.Send(message);
		}

	}

}
