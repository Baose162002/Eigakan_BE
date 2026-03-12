using Eigakan.Application.Interface;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Request.AdPackage;
using Eigakan.Domain.Request.AdSlot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdPackageController : ControllerBase
    {
        private readonly IAdPackageService _adPackageService;
        public AdPackageController(IAdPackageService adPackageService)
        {
            _adPackageService = adPackageService;
        }
		
        [HttpGet("test-redis")]
		public async Task<IActionResult> TestRedis()
		{
			try
			{
				var redis = ConnectionMultiplexer.Connect("Eigakan-cache.redis.cache.windows.net:6380,password=xxxxxxx=,ssl=True,abortConnect=False");
				var db = redis.GetDatabase();
				await db.StringSetAsync("testKey", "Hello from SmarterASP.NET");
				var value = await db.StringGetAsync("testKey");
				return Ok($"Redis test value: {value}");
			}
			catch (Exception ex)
			{
				return BadRequest($"Error: {ex.Message}");
			}
		}

		
        [HttpGet("GetAllAdPackageAsync")]
        [Authorize]
        public async Task<IActionResult> GetAllAdPackageAsync(int page = 1, int pageSize = 10)
        {
            var results = await _adPackageService.GetAllAdPackageAsync(page, pageSize);
			return Ok(new
			{
				results.Total,
				results.AdPackages
			});
		}

        [HttpGet("GetAdPackageById/{id}")]
		[Authorize]
		public async Task<IActionResult> GetAdPackageById(string? id)
        {
            var results = await _adPackageService.GetAdPackageById(id);

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
            }); ;
        }

        [HttpGet("GetAdPackageByQuantity/{quantity}")]
        [Authorize]
        public async Task<IActionResult> GetAdPackageByQuantity(int quantity)
		{
			var results = await _adPackageService.GetAdPackageByQuantity(quantity);

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

        [HttpPut("UpdateAdPackage/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateAdPackage(string id, [FromBody] AdPackageUpdateRequest value)
        {
            var results = await _adPackageService.UpdateAdPackage(id, value);

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

        [HttpPost("CreateAdPackage")]
		[Authorize]
		public async Task<IActionResult> CreateAdPackage([FromBody] AdPackageCreateRequest value)
        {
            var results = await _adPackageService.CreateAdPackage(value);

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
