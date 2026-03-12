using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserEarningController : ControllerBase
	{
		private readonly IUserEarningService _userEarningService;

		public UserEarningController(IUserEarningService userEarningService)
		{
			_userEarningService = userEarningService;
		}

		[HttpGet("userEarning")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllUserEarning(int page = 1, int pageSize = 10)
		{
			var result = await _userEarningService.GetAllUserEarningAsync(page, pageSize);

			return Ok(new
			{
				success = true,
				message = "Get user earning successfully",
				data = new
				{
					totalItems = result.Total,
					totalEarnings = result.TotalEarnings,
					webEarnings = result.WebEarnings,
					finalEarning = result.FinalEarnings,
					userEarnings = result.userEarnings
				}
			});
		}

		[HttpGet("GetUserEarningByLogin")]
		[Authorize]
		public async Task<IActionResult> GetUserEarningDayByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate)
		{
			var (listUserEarning, total, totalEarnings, finalEarnings) = await _userEarningService.GetAllUserEarningByLogin(page, pageSize, startDate, endDate);
			if (listUserEarning == null || listUserEarning.Count == 0)
			{
				return NotFound(new
				{
					success = false,
					message = "No earnings found for this user."
				});
			}

			return Ok(new
			{
				success = true,
				message = "User earnings retrieved successfully.",
				data = new
				{
					total = total,
					totalEarnings = totalEarnings,
					finalEarnings = finalEarnings,
					userEarnings = listUserEarning
				}
			});
		}

		[HttpGet("GetUserEarningByUserId/{userId}")]
		[Authorize]
		public async Task<IActionResult> GetAllUserEarningAsyncByUserId(int page, int pageSize,string userId, DateOnly? startDate, DateOnly? endDate)
		{
			var (listUserEarning, total, totalEarnings, finalEarnings) = await _userEarningService.GetAllUserEarningAsyncByUserId(page, pageSize, startDate, endDate, userId);
			if (listUserEarning == null || listUserEarning.Count == 0)
			{
				return NotFound(new
				{
					success = false,
					message = "No earnings found for this user."
				});
			}

			return Ok(new
			{
				success = true,
				message = "User earnings retrieved successfully.",
				data = new
				{
					total = total,
					totalEarnings = totalEarnings,
					finalEarnings = finalEarnings,
					userEarnings = listUserEarning
				}
			});
		}
	}
}

