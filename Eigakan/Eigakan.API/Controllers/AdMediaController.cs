using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.AdMedia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdMediaController : ControllerBase
    {
        private readonly IAdMediaService _adMediaService;
        public AdMediaController(IAdMediaService adMediaService)
        {
            _adMediaService = adMediaService;
        }

        
        [HttpGet("GetAllAdMedia")]
        [Authorize]
        public async Task<IActionResult> GetAllAdMedia(string? status, int page=1, int pageSize=10)
        {
            var results = await _adMediaService.GetAllListAdMedia(status, page,pageSize);

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

		[HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _adMediaService.GetById(id);

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
        
        [HttpPatch("AdMedia_ApprovedStatus")]
        [Authorize]
        public async Task<IActionResult> ApprovedStatusAdMedia( [FromBody] AdMediaApprovedStatus request)
        {
            var results = await _adMediaService.AdMediaApprovedStatus(request);

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
        
        [HttpPatch("AdMedia_RejectedStatus")]
        [Authorize]
        public async Task<IActionResult> RejectedStatusAdMedia([FromBody]  AdMediaRejectedRequest request)
        {
            var results = await _adMediaService.AdMediaRejectedStatus(request);

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

        [HttpGet("GetRandomAdMedia/{movieId}")]
        public async Task<IActionResult> GetNextAdMedia(string movieId)
        {
            try
            {
              
                var mediaWithSlots = await _adMediaService.GetAdMediaWithPositionsAsync(movieId);

                if (mediaWithSlots == null || !mediaWithSlots.Any())
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "No ad media available"
                    });
                }

                
                return Ok(new
                {
                    Success = true,
                    Message = "Ad media retrieved successfully.",
                    Data = mediaWithSlots
                });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpGet("GetAdMediaByLogin")]
        [Authorize]
        public async Task<IActionResult> GetMediaByUserId([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            var result = await _adMediaService.GetMediaByUserIdAsync(page, pageSize);

            if (result == null)
                return NotFound(new { Success = false, Message = "No media found." });

            return Ok(result);
        }

        [HttpGet("GetMediaStatusExpiredByLogin")]
        [Authorize]
        public async Task<IActionResult> GetMediaStatusExpiredByUserId([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            var result = await _adMediaService.GetMediaStatusEXpiredByUserIdAsync(page, pageSize);

            if (result == null)
                return NotFound(new { Success = false, Message = "No media found." });

            return Ok(result);
        }
    }
}
