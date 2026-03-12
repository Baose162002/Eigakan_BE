using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Genre;
using Eigakan.Domain.Request.Media;
using Eigakan.Domain.Response;
using Eigakan.Domain.Response.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MediaController : ControllerBase
	{
		private readonly IMediaService _mediaService;

		public MediaController(IMediaService mediaService)
		{
			_mediaService = mediaService;
		}
	
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var results = await _mediaService.GetList();
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string? id)
        {
            var results =await _mediaService.GetMediaById(id);
            if (results.Success!= false)
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(string? id, [FromBody] MediaUpdateRequest value)
        {
            var results = await _mediaService.UpdateMedia(id, value);
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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string? id)
        {
            var results = await _mediaService.DeleteMedia(id);
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] MediaCreateRequest value)
        {
            var results = await _mediaService.CreateMedia(value);
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


    }
}
