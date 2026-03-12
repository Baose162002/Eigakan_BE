using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IAdPurchaseItemRepository : IGenericRepository<AdPurchaseItems>
    {

        Task<List<AdPurchaseItems>> GetItemsByTransactionIdAsync(string adPurchaseTransactionId);
        Task<AdPurchaseItems> GetByMediaIdAndHasRemainingViews(string mediaId);
        Task<List<AdPurchaseItems>> GetItemsWithRemainingViewsOlderThan(DateTime threshold);
    }
}
