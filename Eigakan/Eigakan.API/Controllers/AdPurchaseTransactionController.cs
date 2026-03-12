using Eigakan.Application.Interface;
using Eigakan.Domain.Request.AdPurchaseItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdPurchaseTransactionController : ControllerBase
    {
        private readonly IAdPurchaseTransactionService _adPurchaseTransactionService;

        public AdPurchaseTransactionController(IAdPurchaseTransactionService adPurchaseTransactionService)
        {
            _adPurchaseTransactionService = adPurchaseTransactionService;
        }

        [HttpPost("createAdPurchase")]
        [Authorize] 
        public async Task<IActionResult> CreateAdPurchase([FromBody] CreateAdPurchaseRequest request)
        {
            var result = await _adPurchaseTransactionService.CreateAdPurchaseAsync(request);

            if (!result.Success)
                return BadRequest(new { Success = result.Success, Message = result.Message });

            return Ok(new
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpGet("MyHistoryAdPurchaseTransaction")]
        [Authorize]
        public async Task<IActionResult> GetMyHistoryTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _adPurchaseTransactionService.GetListAdPurchaseTransactionForUser(page, pageSize);
            if (!result.Success)
                return BadRequest(result);

            var (data, total) = result.Data;

            return Ok(new
            {
                success = true,
                data,
                total
            });
        }

        [HttpGet("GetAllAdPurchaseTransaction")]
        [Authorize]
        public async Task<IActionResult> GetAllTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _adPurchaseTransactionService.GetListAllAdPurchaseTransaction(page, pageSize);
            if (!result.Success)
                return BadRequest(result);

            var (data, total) = result.Data;

            return Ok(new
            {
                success = true,
                data,
                total
            });
        }

    }
}
