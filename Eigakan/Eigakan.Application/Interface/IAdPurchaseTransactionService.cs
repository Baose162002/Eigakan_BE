using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdPurchaseItem;
using Eigakan.Domain.Request.AdPurchaseTransaction;
using Eigakan.Domain.Response.AdPurchaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IAdPurchaseTransactionService 
    {
        Task<Result<AdPurchaseTransactionGetAllResponse>> CreateAdPurchaseAsync(CreateAdPurchaseRequest request);
        Task<Result<(List<AdPurchaseTransactionGetAllResponse> Data, int Total)>> GetListAllAdPurchaseTransaction(int page, int pageSize);
        Task<Result<(List<AdPurchaseTransactionGetAllResponse> Data, int Total)>> GetListAdPurchaseTransactionForUser(int page, int pageSize);
    }
}
