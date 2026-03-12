using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Domain.Request.AuthRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _userService;
		private readonly ILogger<AuthController> _logger;
		private readonly Webhook _webhook;

		public AuthController(IAuthService userService, ILogger<AuthController> logger, Webhook webhook)
		{
			_userService = userService;
			_logger = logger;
			_webhook = webhook;
		}

		[HttpPost("SignUp")]
		public async Task<IActionResult> SignUp(RegisterRequest user)
		{
			var results = await _userService.Register(user);
			if (results.Success)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
				});
			}

			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginRequest login)
		{
			var results = await _userService.Login(login);
			if (results.Success)
			{
				return Ok(new
				{
					results.Success,
					results.Data,
					results.Message
				});
			}

			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

		[HttpGet("Verify")]
		public async Task<IActionResult> UserVerify(string token)
		{
			var result = await _userService.Verify(token);
			if (!result.Success)
			{
				return BadRequest(new
				{
					result.Success,
					result.Message
				});
			}
			return Ok(new
			{
				result.Success,
				result.Message
			});
		}

		[HttpPost("Forgot-password")]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPassword)
		{
			// Ghi nhật ký khi bắt đầu xử lý yêu cầu
			_logger.LogInformation("Received password reset request for email: {Email}", forgotPassword.Email);

			var results = await _userService.ForgotPassword(forgotPassword);

			// Gửi thông báo về Discord khi bắt đầu hoặc sau khi xử lý thành công
			if (results.Success)
			{
				_logger.LogInformation("Password reset successfully initiated for email: {Email}", forgotPassword.Email);

				// Gửi log về Discord
				await _webhook.Send($"Password reset successfully initiated for email: {forgotPassword.Email}");

				return Ok(new
				{
					results.Success,
					results.Message,
				});
			}

			// Gửi thông báo về Discord khi có lỗi
			_logger.LogError("Failed to initiate password reset for email: {Email}. Error: {ErrorMessage}", forgotPassword.Email, results.Message);

			// Gửi lỗi về Discord
			await _webhook.Send($"Failed to initiate password reset for email: {forgotPassword.Email}. Error: {results.Message}");

			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

		[HttpPost("Reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
		{
			var results = await _userService.ResetPassword(request);
			if (results.Success != false)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
				});
			}
			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

	}
}

