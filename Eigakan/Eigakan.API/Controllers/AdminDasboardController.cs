using Eigakan.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminDasboardController : ControllerBase
	{
		private readonly IAdminDashboardService _adminDashboardService;

		public AdminDasboardController(IAdminDashboardService adminDashboardService)
		{
			_adminDashboardService = adminDashboardService;
		}

		[HttpGet("GetDashboardAdminOverall")]
		public async Task<IActionResult> GetDashboardAdminOverall()
		{
			var results = await _adminDashboardService.DashboardAdminOverall();
			return Ok(new
			{
				results.Success,
				results.Message,
				results.Data
			});
			
		}
	}
}
