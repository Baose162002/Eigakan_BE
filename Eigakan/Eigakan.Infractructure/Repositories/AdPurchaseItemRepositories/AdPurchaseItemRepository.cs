using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.AdPurchaseItemRepositories
{
    public class AdPurchaseItemRepository : GenericBase<Domain.Models.AdPurchaseItems>, IAdPurchaseItemRepository
    {
        public async Task<List<AdPurchaseItems>> GetItemsByTransactionIdAsync(string adPurchaseTransactionId)
        {
            var items =  await Get(
                filter: c => c.AdPurchaseTransactionId == adPurchaseTransactionId,
                orderBy: q => q.OrderByDescending(c => c.CreatedDate)
            );
            return items.ToList();
        }

        public async Task<AdPurchaseItems> GetByMediaIdAndHasRemainingViews(string mediaId)
        {
            return await GetSingle(filter: c => c.AdMediaId == mediaId && c.RemainingViews > 0);

            
        }
        public async Task<List<AdPurchaseItems>> GetItemsWithRemainingViewsOlderThan(DateTime threshold)
        {
            return (await Get(
                filter: x => x.ExpiredDate < threshold && x.RemainingViews > 0
            )).ToList();
        }

        

    }
}
