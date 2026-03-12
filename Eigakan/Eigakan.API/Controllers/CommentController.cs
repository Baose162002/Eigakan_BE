using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Domain.Request.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var results = await _commentService.GetCommentById(id);

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

        [HttpPut("{id}")]
        [Authorize]
        public  async Task<IActionResult> Put(string? id, [FromBody] CommentUpdateRequest value)
        {
            var results = await _commentService.Update(id, value);

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
            var results = await _commentService.Delete(id);

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
        public async Task<IActionResult> Post([FromBody] CommentCreateRequest value)
        {
            var results = await _commentService.Create(value);

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
