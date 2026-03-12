using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Application.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MovieEarningController : ControllerBase
	{
		private readonly IMovieEarningService _movieEarningService;

		public MovieEarningController(IMovieEarningService movieEarningService)
		{
			_movieEarningService = movieEarningService;
		}

		[HttpGet("movieEarning")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllMovieEarning(int page = 1, int pageSize = 10)
		{
			var result = await _movieEarningService.GetAllMovieEarningAsync(page, pageSize);

			return Ok(new
			{
				success = true,
				message = "Get movie earning successfully",
				data = new
				{
					totalItems = result.Total,
					totalView = result.TotalView,
					totalEarnings = result.TotalEarnings,
					totalEarningsMovieContract = result.TotalEarningsMovieContract,
					movieEarning = result.MovieEarning
				}
			});
		}


		[HttpGet("GetMovieEarningByMovieId/{movieId}")]
		[Authorize]
		public async Task<IActionResult> GetAllMovieEarningByMovieId(int page, int pageSize, string movieId, DateOnly? startDate, DateOnly? endDate)
		{
			var (movieEarningMovieId, total, totalEarning) = await _movieEarningService.GetAllMovieEarningByMovieId(page, pageSize, startDate, endDate, movieId);

			if (movieEarningMovieId == null || movieEarningMovieId.Count == 0)
			{
				return NotFound(new
				{
					success = false,
					message = "No earnings found for this movie."
				});
			}

			return Ok(new
			{
				success = true,
				message = "Movie earnings retrieved successfully.",
				data = new
				{
					totalItems = total,
					totalEarnings = totalEarning,
					movieEarningMovieId,
				}
			});
		}


	}
}
