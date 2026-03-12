using Ardalis.GuardClauses;
using Discord;
using Eigakan.Application.Helper.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace Eigakan.Application.Helper
{
	[JsonObject]
	public class Webhook
	{
		private readonly HttpClient _httpClient;
		private readonly string _webhookUrl;

		[JsonProperty("content")]
		public string Content { get; set; } = "";

		[JsonProperty("username")]
		public string Username { get; set; } = "";

		[JsonProperty("avatar_url")]
		public string AvatarUrl { get; set; } = "";

		// ReSharper disable once InconsistentNaming
		[JsonProperty("tts")]
		public bool IsTTS { get; set; }

		[JsonProperty("embeds")]
		public List<Embed> Embeds { get; set; } = new List<Embed>();

		// Constructor sử dụng IOptions<DiscordWebhookUrls>
		public Webhook(IOptions<DiscordWebhookUrls> optionsAccessor)
		{
			Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
			Guard.Against.NullOrEmpty(optionsAccessor.Value.AdminUpdates, "AdminUpdates");

			_httpClient = new HttpClient();
			_webhookUrl = optionsAccessor.Value.AdminUpdates;
		}

		// Constructor sử dụng URL webhook trực tiếp
		public Webhook(string webhookUrl)
		{
			Guard.Against.NullOrEmpty(webhookUrl, nameof(webhookUrl));  // Kiểm tra URL webhook

			_httpClient = new HttpClient();
			_webhookUrl = webhookUrl;
		}

		// Constructor với ID và Token, sử dụng URL webhook cứng
		public Webhook(ulong id, string token)
			: this($"https://discord.com/api/webhooks/1326070733650137139/qG7-RocMHVj0CFlw3vAr2YGE2Ou7QJfOA9jwzL7TgZj0JJTgYkSUBFg_yfWMWPYhjLs-")
		{
		}

		// Gửi dữ liệu đến webhook
		public async Task<HttpResponseMessage> Send()
		{
			if (string.IsNullOrEmpty(Username) || Username.Length > 80)
			{
				throw new ArgumentException("Username must be between 1 and 80 characters in length.");
			}

			var content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(_webhookUrl, content);
			var responseContent = await response.Content.ReadAsStringAsync();
			Console.WriteLine(responseContent);  // In ra nội dung của response
			return response;
		}

		// Gửi dữ liệu đến webhook với các tham số tùy chỉnh (chỉnh sửa con bot)
		public async Task<HttpResponseMessage> Send(string content, string username = "Bảo", string avatarUrl = "https://res.cloudinary.com/dn8bn2sty/image/upload/v1736246244/79721f01f411484f1100_ddtgtf.jpg", bool isTTS = false, IEnumerable<Embed>? embeds = null)
		{
			Content = content;
			Username = username;
			AvatarUrl = avatarUrl;
			IsTTS = isTTS;
			Embeds.Clear();
			if (embeds != null)
			{
				Embeds.AddRange(embeds);
			}

			return await Send();
		}
	}
}
