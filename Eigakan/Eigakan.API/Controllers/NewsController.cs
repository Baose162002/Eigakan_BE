using Microsoft.AspNetCore.Mvc;
using Eigakan.Domain.Request.News;
using Eigakan.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Eigakan.Domain.Enum;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await _newsService.GetList();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _newsService.GetNewsById(id);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }


		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetNewsByUser(string userId)
		{
			var result = await _newsService.GetNewsByUserId(userId);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}

        [HttpPut("{id}")]
        [Authorize(Roles = "MANAGER")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateNewsRequest request)
        {
            var result = await _newsService.UpdateNews(id, request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MANAGER")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _newsService.DeleteNews(id);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

		[HttpPost]
		[Authorize(Roles = "MANAGER")]
		public async Task<IActionResult> Create([FromBody] CreateNewsRequest request)
		{
			var result = await _newsService.CreateNews(request);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}
	}
}