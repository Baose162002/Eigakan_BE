using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RolesController : ControllerBase
	{
		private readonly IRoleService _roleService;

		public RolesController(IRoleService roleService)
        {
			_roleService = roleService;
		}

		[HttpGet("GetAllRole")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllRoleAsync(int page = 1, int pageSize = 10)
		{
			var result = await _roleService.GetAllRoleAsync(page, pageSize);

			return Ok(new
			{
				result.Total,
				result.Roles
			});
		}
    }
}
