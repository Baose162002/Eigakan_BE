using Eigakan.Application.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class BunnyStreamUploadService
	{
		private readonly HttpClient _httpClient;
		private readonly BunnyStreamSettings _settings;

		public BunnyStreamUploadService(HttpClient httpClient, IOptions<BunnyStreamSettings> settings)
		{
			_httpClient = httpClient;
			_settings = settings.Value;
		}

		public async Task<string?> UploadVideoAsync(string fileName)
		{
			//  Tạo video trên Bunny Stream trước khi upload
			var createVideoUrl = $"https://video.bunnycdn.com/library/{_settings.LibraryId}/videos";
			var createRequest = new HttpRequestMessage(HttpMethod.Post, createVideoUrl);
			createRequest.Headers.Add("AccessKey", _settings.ApiKey);
			createRequest.Content = new StringContent("{\"title\": \"" + fileName + "\"}", System.Text.Encoding.UTF8, "application/json");

			var createResponse = await _httpClient.SendAsync(createRequest);
			if (!createResponse.IsSuccessStatusCode)
				return null;

			var createContent = await createResponse.Content.ReadAsStringAsync();
			var createJson = JsonNode.Parse(createContent);
			var videoId = createJson?["guid"]?.ToString();
			if (string.IsNullOrEmpty(videoId))
				return null;

			return videoId;

		}

		//public async Task<string?> UploadVideoAsync(IFormFile file)
		//{
		//	if (file == null || file.Length == 0)
		//		return null;

		//	// 1️⃣ Tạo video trên Bunny Stream
		//	var createVideoUrl = $"https://video.bunnycdn.com/library/{_settings.LibraryId}/videos";
		//	var createRequest = new HttpRequestMessage(HttpMethod.Post, createVideoUrl);
		//	createRequest.Headers.Add("AccessKey", _settings.ApiKey);
		//	createRequest.Content = new StringContent($"{{\"title\": \"{file.FileName}\"}}", Encoding.UTF8, "application/json");

		//	var createResponse = await _httpClient.SendAsync(createRequest);
		//	if (!createResponse.IsSuccessStatusCode)
		//		return null;

		//	var createContent = await createResponse.Content.ReadAsStringAsync();
		//	var createJson = JsonNode.Parse(createContent);
		//	var videoId = createJson?["guid"]?.ToString();
		//	if (string.IsNullOrEmpty(videoId))
		//		return null;

		//	// 2️⃣ Upload video lên Bunny Stream
		//	var uploadUrl = $"https://video.bunnycdn.com/library/{_settings.LibraryId}/videos/{videoId}";
		//	using var fileContent = new StreamContent(file.OpenReadStream());
		//	fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // ✅ Đúng

		//	var uploadRequest = new HttpRequestMessage(HttpMethod.Put, uploadUrl) // ✅ Dùng PUT thay vì POST
		//	{
		//		Content = fileContent
		//	};
		//	uploadRequest.Headers.Add("AccessKey", _settings.ApiKey);

		//	var uploadResponse = await _httpClient.SendAsync(uploadRequest);
		//	if (!uploadResponse.IsSuccessStatusCode)
		//		return null;

		//	return $"https://iframe.mediadelivery.net/embed/{_settings.LibraryId}/{videoId}";
		//}
	}
}
