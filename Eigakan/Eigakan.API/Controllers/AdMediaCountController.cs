using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.AdMedia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdMediaCountController : ControllerBase
	{
		private readonly IAdMediaCountService _adMediaCountService;
		private readonly ILogger<AdMediaCountController> _logger;

		public AdMediaCountController(IAdMediaCountService adMediaCountService, ILogger<AdMediaCountController> logger) 
		{
			_adMediaCountService = adMediaCountService;
			_logger = logger;
		}
		
		[HttpGet("GetAdMediaCountByAdMediaId/{adMediaId}")]
		public async Task<IActionResult> GetAdMediaCountByAdMediaId(string adMediaId)
		{
			var result = await _adMediaCountService.GetAdMediaCountByAdMediaId(adMediaId);

			if (!result.Success)
			{
				_logger.LogError($"Error getting AdMediaCount for adMediaId={adMediaId}: {result.Message}");
				return BadRequest(new
				{
					result.Success,
					result.Message
				});
			}

			return Ok(new
			{
				result.Success,
				result.Data
			});
		}
		
		[HttpGet("StatisticAdMediaCount/{adMediaId}")]
		public async Task<IActionResult> StatisticAdMediaCount(string adMediaId)
		{
			var result = await _adMediaCountService.StatisticAdMediaCount(adMediaId);

			return Ok(new
			{
				result,
			});
		}
		
		//[HttpPost("IncreaseAdMediaCount")]
		//public async Task<IActionResult> IncreaseAdMediaCount(AdClickCountCreateRequest adClickCount)
		//{
		//	// Log request để debug
		//	_logger.LogInformation($"IncreaseAdMediaCount request: {JsonSerializer.Serialize(adClickCount)}");
			
		//	try
		//	{
		//		var results = await _adMediaCountService.IncreaseAdMediaCount(adClickCount);
		//		if (results.Success)
		//		{
		//			return Ok(new
		//			{
		//				results.Success,
		//				results.Message,
		//				results.Data
		//			});
		//		}
		//		else
		//		{
		//			_logger.LogError($"Error in IncreaseAdMediaCount: {results.Message}");
		//			return BadRequest(new
		//			{
		//				results.Success,
		//				results.Message
		//			});
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, $"Unhandled exception in IncreaseAdMediaCount");
		//		return StatusCode(500, new
		//		{
		//			Success = false,
		//			Message = $"Server error: {ex.Message}"
		//		});
		//	}
		//}

        [HttpPost("IncreaseAdMediaCount/{mediaId}")]
        public async Task<IActionResult> CreateCountAdMediaAsync(string mediaId)
        {
            var result = await _adMediaCountService.CreateCountAdMediaAsync(mediaId);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }



    }
} 