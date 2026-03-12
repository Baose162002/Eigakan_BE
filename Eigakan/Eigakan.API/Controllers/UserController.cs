using Eigakan.Application.Interface;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Request.UserRequest;
using Eigakan.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet("GetAllUser"), Authorize]
		public async Task<IActionResult> GetAllUser(int page = 1, int pageSize = 10, string? status = null,string? name = null,string? roleName=null)
		{
			var result = await _userService.GetAllUserAsync(page, pageSize, status, name, roleName);
			
			return Ok(new
			{
				result.Total,
				result.Users			
			});
		}

		[HttpGet("GetUserByLogin"), Authorize]
		public async Task<IActionResult> GetUserByLogin()
		{
			var result = await _userService.GetUserByLogin();
			return Ok(new
			{
				result.Success,
				result.Message,
				result.Data
			});
		}

		[HttpGet("GetUserById/{id}"), Authorize]
		public async Task<IActionResult> GetUserById(string id)
		{
			var user = await _userService.GetUserById(id);
			return Ok(user);
		}

		[HttpGet("GetUserByEmail/{email}"), Authorize]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var user = await _userService.GetUserByEmail(email);
			return Ok(user);
		}
		
		[HttpPut("UpdateUser/{id}"), Authorize]
		public async Task<IActionResult> UpdateUser(string id, UserUpdateRequest userUpdateRequest)
		{
			var results = await _userService.UpdateUser(id, userUpdateRequest);
			if (results.Success != false)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
					results.Data
				});
			}
			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

		[HttpPatch("ActiveDeactive_User") , Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> ActiveDeactiveUser(UserStatusRequest userStatusRequest)
		{
			var results = await _userService.ChangeStatusUser(userStatusRequest);
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

		[HttpPost("CreateUserByRegister"), Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> CreateUserByRegister(UserCreateRequest userCreateRequest)
		{
			var results = await _userService.CreateUserByRegister(userCreateRequest);
			if (results.Success != false)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
					results.Data
				});
			}
			return BadRequest(new
			{
				results.Success,
				results.Message
			});

		}

		[HttpPost("CreateUser"), Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> CreateUser(UserCreateRequest userCreateRequest)
		{
			var results = await _userService.CreateUser(userCreateRequest);
			if (results.Success != false)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
					results.Data
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
