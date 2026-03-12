using Eigakan.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NotificationController : ControllerBase
	{
		private readonly NotificationService _notificationService;

		public NotificationController(NotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		[HttpPost("send")]
		public async Task<IActionResult> SendNotification()
		{
			// Gọi service để gửi thông báo
			await _notificationService.SendNotificationAsync();

			// Trả về kết quả
			return Ok("Thông báo đã được gửi thành công.");
		}
	}
}