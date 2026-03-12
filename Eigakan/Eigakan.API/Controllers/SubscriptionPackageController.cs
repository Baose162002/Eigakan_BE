using Eigakan.Application.Interface;
using Eigakan.Domain.Request.SubscriptionPackageRequest;
using Eigakan.Domain.Response.SubscriptionPackageResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPackageController : ControllerBase
    {
        private readonly ISubscriptionPackageService _subscriptionPackageService;
        private readonly ILogger<SubscriptionPackageController> _logger;

        public SubscriptionPackageController(
            ISubscriptionPackageService subscriptionPackageService,
            ILogger<SubscriptionPackageController> logger)
        {
            _subscriptionPackageService = subscriptionPackageService;
            _logger = logger;
        }

        [HttpGet]   
        public async Task<IActionResult> GetAllSubscriptionPackages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _subscriptionPackageService.GetAllSubscriptionPackageAsync(page, pageSize);
            if (!result.Success)
                return BadRequest(result);

            return Ok(new
            {
                Success = result.Success,
                Message = result.Message,
                Data = new
                {
                    Total = result.Data.Total,
                    Subscriptionpackage = result.Data.SubscriptionPackages,
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscriptionPackageById(string id)
        {
            var result = await _subscriptionPackageService.GetSubscriptionPackageById(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
		[Authorize(Roles = "MANAGER")]
		public async Task<IActionResult> UpdateSubscriptionPackage(string id, [FromBody] SubscriptionPackageUpdateRequest request)
        {
            var result = await _subscriptionPackageService.UpdateSubscriptionPackageAsync(id, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("{id}")]
		[Authorize(Roles = "MANAGER")]
		public async Task<IActionResult> UpdateSubscriptionPackageStatus(string id)
        {
            var result = await _subscriptionPackageService.UpdateSubscriptionPackageStatusAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

		[HttpPost]
		[Authorize(Roles = "MANAGER")]
		public async Task<IActionResult> CreateSubscriptionPackage([FromBody] SubscriptionPackageCreateRequest request)
		{
			var result = await _subscriptionPackageService.CreateSubscriptionPackageAsync(request);
			if (!result.Success)
				return BadRequest(result);

			return Ok(result);
		}

	}
}
