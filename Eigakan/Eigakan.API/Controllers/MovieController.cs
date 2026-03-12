using Eigakan.Application.Interface;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Request.Movie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        
        public MovieController(IMovieService movieService)
        {
             _movieService= movieService;
        }
        
        [HttpGet("GetListAllMovie")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetListAllMovie(int pageNumber=1, int pageSize=10, string? genreFilter = null, string? nameFilter=null, string? statusFilter = null)
        {
            var movies = await _movieService.GetListAllMovie(pageNumber, pageSize,genreFilter, nameFilter,statusFilter);
			
            return Ok(new
			{
				movies.Total,
				movies.movies
			});
		}

		[HttpGet("GetListMovieActive")]
		public async Task<IActionResult> GetListMovieActive(int pageNumber = 1, int pageSize = 10, string? genreFilter = null, string? nameFilter = null, string? statusFilter = null)
		{
			var movies = await _movieService.GetListMovieActive(pageNumber, pageSize, genreFilter, nameFilter, statusFilter);
			return Ok(new
			{
				movies.Total,
				movies.movies
			});
		}

		[HttpGet("GetListMovieByLogin")]
        [Authorize]
		public async Task<IActionResult> GetListMovieByLogin(int pageNumber = 1, int pageSize = 10, string? genreFilter = null, string? nameFilter = null, string? statusFilter = null)
		{
			var movies = await _movieService.GetListAllMovieByLogin(pageNumber, pageSize, genreFilter, nameFilter, statusFilter);
			return Ok(new
			{
				movies.Total,
                movies.ActiveMovie,
				movies.movies
			});
		}

		[HttpGet("GetListMovieByUserId")]
		[Authorize]
		public async Task<IActionResult> GetListAllMovieByUserId(string userId, int pageNumber = 1, int pageSize = 10, string? genreFilter = null, string? nameFilter = null, string? statusFilter = null)
		{
			var movies = await _movieService.GetListAllMovieByUserId(userId,pageNumber, pageSize, genreFilter, nameFilter, statusFilter);
			return Ok(new
			{
				movies.Total,
				movies.movies
			});
		}

		[HttpGet("GetMovieById/{id}")]
        public async Task<IActionResult> GetMovieById(string id)
        {

            var results = await _movieService.GetByMovieIdClear(id);
            if (results.Success != false)
            {
                return Ok(new
                {
                    results.Success,
                    results.Data
                });
            }
            return BadRequest(new
            {
                results.Success,
                results.Message
            });
        }
    
        [HttpPut("UpdateMovie/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateMovie(string id, [FromBody] UpdateMovieRequest value)
        {
            var results = await _movieService.UpdateMovie(id, value);
            if (results.Success != false)
            {
                return Ok(new
                {
                    results.Success,
                    results.Message,
                });
            }
            return BadRequest(new
            {
                results.Success,
                results.Message
            });
        }

        [HttpPatch("AcceptedMovie")]
        [Authorize]
        public async Task<IActionResult> AcceptedMovie(AcceptedMovieRequest acceptedMovieRequest)
        {
            var results = await _movieService.AcceptedMovie(acceptedMovieRequest);
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

		[HttpPatch("AcceptedMovieNotContract")]
		[Authorize]
		public async Task<IActionResult> AcceptedMovieNotContract(AcceptedMovieRequest acceptedMovieRequest)
		{
			var results = await _movieService.AcceptedMovieNotContract(acceptedMovieRequest);
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


		[HttpPatch("RejectedMovie")]
		[Authorize]
		public async Task<IActionResult> RejectedMovie(RejectedMovieRequest rejectedMovieRequest)
		{
			var results = await _movieService.RejectedMovie(rejectedMovieRequest);
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
		
        [HttpPatch("ArchivedMovie/{id}")]
        [Authorize]
        public async Task<IActionResult> ArchivedMovie(string id)
        {
            var results = await _movieService.ArchivedMovie(id);
            if (results.Success != false)
            {
                return Ok(new
                {
                    results.Success,
                    results.Message,
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
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest value)
        {
            var results = await _movieService.CreateMovie(value);
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
