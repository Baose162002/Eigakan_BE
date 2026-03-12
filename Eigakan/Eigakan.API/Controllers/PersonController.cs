using Eigakan.Application.Interface;
using Eigakan.Domain.Request.Person;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllPerson(int pageNumber=1, int pageSize=10,string? name=null,bool? gender=null)
        {
          

            var person = await _personService.GetList(pageNumber, pageSize, name, gender);
            if (person == null)
            {
                return NotFound("No persons found.");
            }

            //     var movies = await _movieService.GetListMovie();

            return Ok(person);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonById(string id)
        {
            var results = await _personService.GetPersonById(id);
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
        public async Task<IActionResult> UpdatePerson(string? id, [FromBody] PersonCreateRequest value)
        {
            var results = await _personService.UpdatePerson(id,value);
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
            var results = await _personService.DeletePerson(id);
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
        public async Task<IActionResult> CreatePerson([FromBody] PersonCreateRequest value)
        {
            var results = await _personService.CreatePerson(value);
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
