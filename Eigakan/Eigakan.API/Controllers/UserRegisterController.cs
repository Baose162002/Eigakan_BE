using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.UserRegisterRequest;
using Eigakan.Domain.Request.UserRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserRegisterController : ControllerBase
	{
		private readonly IUserRegisterService _userRegisterService;

		public UserRegisterController(IUserRegisterService userRegisterService) 
		{
			_userRegisterService = userRegisterService;
		}

		[HttpGet("userRegister")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllUserRegister(int page = 1, int pageSize = 10,string? status=null,string? name=null)
		{
			var result = await _userRegisterService.GetAllUserRegisterAsync(page, pageSize, status, name);
			return Ok(new
			{
				result.Total,
				result.Users
			});
		}

		[HttpGet("userRegisterById/{id}")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetUserById(string id)
		{
			var user = await _userRegisterService.GetUserRegisterById(id);
			return Ok(user);
		}

		[HttpGet("userRegisterByEmail/{email}")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var user = await _userRegisterService.GetAllUserRegisterAsyncByEmail(email);
			return Ok(user);
		}

		[HttpPatch("Accepted_UserRegister")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> AcceptedUserRegister(AcceptedUserRegisterRequest acceptedUserRegisterRequest )
		{
			var results = await _userRegisterService.AcceptedUserRegister(acceptedUserRegisterRequest);
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

		[HttpPatch("Rejected_UserRegister")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> RejectedUserRegister(RejectedUserRegisterRequest rejectedUserRegisterRequest)
		{
			var results = await _userRegisterService.RejectedUserRegister(rejectedUserRegisterRequest);
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

		[HttpPost("CreateUserRegister")]
		public async Task<IActionResult> CreateUserRegister (UserRegisterCreateRequest userRegisterCreateRequest)
		{
			var results = await _userRegisterService.CreateUserRegister(userRegisterCreateRequest);
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
