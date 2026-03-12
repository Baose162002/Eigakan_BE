using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.MovieHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MovieCountController : ControllerBase
	{
		private readonly IMovieCountService _movieCountService;

		public MovieCountController(IMovieCountService movieCountService) 
		{
			_movieCountService = movieCountService;
		}

		[HttpGet("GetMovieCountByMovieId/{movieId}")]
		public async Task<IActionResult> GetMovieHistoryByLogin(string movieId)
		{
			var result = await _movieCountService.GetMovieCountByMovieId(movieId);

			return Ok(new
			{
				result.Success,
				result.Data
			});
		}

		[HttpGet("StatisticMovieCount/{movieId}")]
		public async Task<IActionResult> StatisticMovieCount(string movieId)
		{
			var result = await _movieCountService.GetMovieViewStatistics(movieId);

			return Ok(new
			{
				result,
			});
		}

		[HttpPost("IncreaseMovieCount")]
		public async Task<IActionResult> IncreaseMovieCount(MovieHistoryCreateRequest movieCount)
		{
			var results = await _movieCountService.IncreaseMovieCount(movieCount);
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
