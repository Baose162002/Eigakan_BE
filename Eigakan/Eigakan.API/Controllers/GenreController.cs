using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Genre;
using Eigakan.Domain.Response.Genre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        public GenreController(IGenreService genreService)
        {
              _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGerne()
        {

            var results = await _genreService.GetList();
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
        public async Task<IActionResult> GetGernById(string? id)
        {
            var results= await _genreService.GetGenreById(id);
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateGerne(string? id, [FromBody] GenreUpdateRequest value)
        {
            var results = await _genreService.UpdateGenre(id, value);

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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string? id)
        {
            var results= await _genreService.DeleteGenre(id);

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
                results.Message,
                results.Data
            }); ;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateGerne([FromBody] CreateGenreRequest value)
        {
            var results = await _genreService.CreateGenre(value);
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
