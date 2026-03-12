using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IAdPurchaseTransactionRepository :IGenericRepository<AdPurchaseTransaction>
    {
        Task<AdPurchaseTransaction> GetAdPurchaseTransactionById(string id);
        Task<List<AdPurchaseTransaction>> GetAdPurchaseTransactionByUserId(string userId);
        Task<List<AdPurchaseTransaction>> GetAllAdPurchaseTransaction(int page, int pageSize);
        Task<List<AdPurchaseTransaction>> GetAdPurchaseTransactionByUserIdPaging(string userId, int page, int pageSize);
        Task<int> CountAllAdPuchaseTransactionByUserIdAsync(string userId);
        Task<int> CountAllAdPuchaseTransactionAsync();
    }
}

