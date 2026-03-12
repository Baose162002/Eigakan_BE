using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface ISubscriptionPurchaseRepository : IGenericRepository<SubscriptionPurchase>
    {
        Task<SubscriptionPurchase> GetSubscriptionPurchaseById(string id);
        Task<List<SubscriptionPurchase>> GetExpiredSubscriptions();
        Task<List<SubscriptionPurchase>> GetSubscriptionPurchaseUserById(string userId, int page, int pageSize);
        Task<SubscriptionPurchase> GetLatestUserSubscription(string userId);
        Task<int> CountAllSubscriptionPackageAsync();
        Task<List<SubscriptionPurchase>> GetAllSubscriptionPurchase(
       int page, int pageSize, string? id, DateTime? startDate, DateTime? endDate, DateTime? expiredDate,
    decimal? totalPrice, string? status, string? subscriptionId, string? userId);
		Task<List<SubscriptionPurchase>> GetAllSubscriptionPurchaseNoPaging();

	}
}
