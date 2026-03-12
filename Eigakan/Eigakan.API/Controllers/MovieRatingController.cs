using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.MovieRating;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieRatingController : ControllerBase
    {
        
        private readonly IMovieRatingService _movieRatingService;
        public MovieRatingController(IMovieRatingService movieRatingService)
        {
            _movieRatingService = movieRatingService;
        }
        
 
        [HttpGet("GetRatingByLogin")]
        [Authorize]
        public async Task<IActionResult> GetRatingByLogin(string movieId)
        {
            var results = await _movieRatingService.GetMovieRatingByLogin(movieId);

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
       
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] MovieRatingCreateRequest value)
        {
            var results = await _movieRatingService.Rating(value);

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
