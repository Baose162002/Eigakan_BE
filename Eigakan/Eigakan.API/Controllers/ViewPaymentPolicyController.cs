using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.ViewPaymentPolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ViewPaymentPolicyController : ControllerBase
	{
		private readonly IViewPaymentPolicyService _viewPaymentPolicyService;

		public ViewPaymentPolicyController(IViewPaymentPolicyService viewPaymentPolicyService) 
		{
			_viewPaymentPolicyService = viewPaymentPolicyService;
		}

		[HttpGet("GetAllViewPaymentPolicy")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllViewPaymentPolicy(int page = 1, int pageSize = 10)
		{
			var result = await _viewPaymentPolicyService.GetAllViewPaymentPolicyAsync(page, pageSize);
			return Ok(new
			{
				result.Total,
				result.Policies
			});
		}

		[HttpGet("GetViewPaymentPolicyById/{id}")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetUserById(string id)
		{
			var policy = await _viewPaymentPolicyService.GetViewPaymentPolicyById(id);
			return Ok(policy);
		}

		[HttpGet("GetViewPaymentPolicyActive")]
		public async Task<IActionResult> GetViewPaymentPolicyActive()
		{
			var policy = await _viewPaymentPolicyService.GetViewPaymentPolicyActive();
			return Ok(policy);
		}

		[HttpGet("GetListPolicyPendingAndWaiting")]
		public async Task<IActionResult> GetListPolicyPendingAndWaiting()
		{
			var policy = await _viewPaymentPolicyService.GetListPolicyPendingAndWaiting();
			return Ok(policy);
		}

		[HttpPut("UpdateViewPaymentPolicy/{id}")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> UpdatePolicy(string id ,ViewPaymentPolicyUpdateRequest viewPaymentPolicyUpdateRequest)
		{
			var results = await _viewPaymentPolicyService.UpdatePolicy(id, viewPaymentPolicyUpdateRequest);
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

		[HttpPost("CancelPolicy")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> CancelPolicy()
		{
			var results = await _viewPaymentPolicyService.CancelPolicy();
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

		[HttpPost("CreateViewPaymentPolicy")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> CreateViewPaymentPolicy(ViewPaymentPolicyCreateRequest viewPaymentPolicyCreateRequest )
		{
			var results = await _viewPaymentPolicyService.CreateViewPaymentPolicy(viewPaymentPolicyCreateRequest);
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
