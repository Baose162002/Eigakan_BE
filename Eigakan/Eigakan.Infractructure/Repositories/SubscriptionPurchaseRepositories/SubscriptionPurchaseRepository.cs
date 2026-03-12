using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.SubscriptionPurchaseRepositories
{
    public class SubscriptionPurchaseRepository : GenericBase<SubscriptionPurchase>, ISubscriptionPurchaseRepository
    {
        public async Task<List<SubscriptionPurchase>> GetExpiredSubscriptions()
        {
            var subscriptionPurchase = await Get();
            return subscriptionPurchase
                .Where(sp => sp.ExpiredDate < DateTime.UtcNow && sp.Status != "Expired")
                .ToList();
        }
        
        public async Task<List<SubscriptionPurchase>> GetAllSubscriptionPurchase(
        int page, int pageSize, string? id, DateTime? startDate, DateTime? endDate, DateTime? expiredDate,
        decimal? totalPrice, string? status, string? subscriptionId, string? userId)
        {
            var subscriptionPurchase = await Get(
                includeProperties: "User",
				orderBy: q => q.OrderByDescending(u => u.PurchaseDate),
				filter: q => (string.IsNullOrEmpty(id) || q.Id == id) &&
                             (!startDate.HasValue || (q.PurchaseDate.HasValue && q.PurchaseDate.Value.Date >= startDate.Value.Date)) &&
                             (!endDate.HasValue || (q.PurchaseDate.HasValue && q.PurchaseDate.Value.Date <= endDate.Value.Date)) &&
                             (!expiredDate.HasValue || (q.ExpiredDate.HasValue && q.ExpiredDate.Value.Date == expiredDate.Value.Date)) &&
                             (!totalPrice.HasValue || q.TotalPrice == totalPrice) &&
                             (string.IsNullOrEmpty(status) || q.Status == status) &&
                             (string.IsNullOrEmpty(subscriptionId) || q.SubscriptionId == subscriptionId) &&
                             (string.IsNullOrEmpty(userId) || q.UserId == userId),
                pageIndex: page,
                pageSize: pageSize
            );

            return subscriptionPurchase.ToList();
        }

		public async Task<List<SubscriptionPurchase>> GetAllSubscriptionPurchaseNoPaging()
		{
			var subscriptionPurchase = await Get(
				includeProperties: "User",
				orderBy: q => q.OrderByDescending(u => u.PurchaseDate)		
			);

			return subscriptionPurchase.ToList();
		}

		public async Task<int> CountAllSubscriptionPackageAsync()
        {
            return await CountAsync();
        }
        
        public async Task<SubscriptionPurchase> GetSubscriptionPurchaseById(string id)
        {
            return await GetSingle(filter: c => c.Id == id);
        }
        
        public async Task<List<SubscriptionPurchase>> GetSubscriptionPurchaseUserById(string userId, int page, int pageSize)
        {
            var subscriptionPurchase = await Get(filter: u => u.UserId == userId, pageIndex: page,
                pageSize: pageSize);
            return subscriptionPurchase.OrderByDescending(u => u.PurchaseDate).ToList();
        }

        public async Task<SubscriptionPurchase> GetLatestUserSubscription(string userId)
        {
            var subscriptions = await Get(filter: c => c.UserId == userId && c.Status == "Active");

            return subscriptions
                .OrderByDescending(sp => sp.ExpiredDate) 
                .FirstOrDefault(); 
        }

    }
}
