using Eigakan.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _userWalletService;
        public UserWalletController(IUserWalletService userWalletService)
        {
            _userWalletService = userWalletService;
        }
        
        [HttpGet("GetUserWalletByLogin")]
        [Authorize]
        public async Task<IActionResult> GetMyWalletByUser()
        {
            var result = await _userWalletService.GetUserWalletById();

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return Ok(result.Data);
        }
    }
}
