using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.AdPurchaseItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IAdPurchaseItemService
    {
        Task<(List<AdPurchaseItemGetAllResponse> Items, int Total)> GetUserAdPurchaseHistoryAsync(int page, int pageSize);
        Task<(List<AdPurchaseItemGetAllResponse> Items, int Total, decimal? totalConsumed, decimal? totalPurchased)> GetAllAdPurchaseHistoryAsync(int page, int pageSize);
        Task<List<AdPurchaseItemGetAllResponse>> GetAllAdPurchaseItemById(string id);


	}
}
