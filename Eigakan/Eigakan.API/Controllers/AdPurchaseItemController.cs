using Eigakan.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdPurchaseItemController : ControllerBase
    {
        private readonly IAdPurchaseItemService _adPurchaseItemService;

        public AdPurchaseItemController(IAdPurchaseItemService adPurchaseItemService)
        {
            _adPurchaseItemService = adPurchaseItemService;
        }

        [HttpGet("GetAdPurchaseItemsByLogin")]
        [Authorize]
        public async Task<IActionResult> GetUserPurchaseHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (items, total) = await _adPurchaseItemService.GetUserAdPurchaseHistoryAsync(page, pageSize);

            return Ok(new
            {
                success = true,
				message = "Success",
				total = total,
                data = items
            });
        }

        [HttpGet("GetAllAdPurchaseItems")]
        [Authorize]
        public async Task<IActionResult> GetAllAdPurchaseHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (items, total, totalConsumed, totaltotalPurchased) = await _adPurchaseItemService.GetAllAdPurchaseHistoryAsync(page, pageSize);

            return Ok(new
            {
                success = true,
                message = "Success",
				total = total,
                totalConsumed = totalConsumed,
                totalPurchased = totaltotalPurchased,   
				data = items,  
                 
            });
        }


        [HttpGet("GetAllAdPurchaseItemsById")]
        [Authorize]
        public async Task<IActionResult> GetAllAdPurchaseItemById([FromQuery] string id)
		{
			var items = await _adPurchaseItemService.GetAllAdPurchaseItemById(id);

			return Ok(new
			{
				success = true,
				message = "Success",
				data = items
			});
		}


    }
}
