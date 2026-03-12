using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.MovieHistory;
using Eigakan.Domain.Request.UserRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MovieHistoryController : ControllerBase
	{
		private readonly IMovieHistoryService _movieHistoryService;

		public MovieHistoryController(IMovieHistoryService movieHistoryService) 
		{
			_movieHistoryService = movieHistoryService;
		}

		[HttpGet("GetMovieHistoryByLogin"), Authorize]
		public async Task<IActionResult> GetMovieHistoryByLogin(int page = 1, int pageSize = 10)
		{
			var result = await _movieHistoryService.GetAlMovieHistoryAsync(page, pageSize);

			return Ok(new
			{
				result.Total,
				result.movieHistories
			});
		}

		[HttpPost("CreateMovieHistory"), Authorize]
		public async Task<IActionResult> CreateMovieHistory(MovieHistoryCreateRequest movieHistoryCreateRequest)
		{
			var results = await _movieHistoryService.CreateMovieHistory(movieHistoryCreateRequest);
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
