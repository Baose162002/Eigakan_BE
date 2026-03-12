using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response;
using Eigakan.Domain.Response.SubscriptionPurchaseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface ISubscriptionPurchaseService
    {
        Task<Result<bool>> SavePurchaseAsync(SubscriptionPurchase subscriptionPurchase);
        Task<Result<UserGetAllResponse>> UpdateStatusUserSubscriptionPurchase(string id);
        Task UpdateExpiredSubscriptions();
        Task<SubscriptionPurchase> GetLatestUserSubscription(string userId);
        Task<Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total, int ActiveSubscriptionCount, decimal totalEarnings)>>
            GetAllSubscriptionPurchaseAsync(
                    int page, int pageSize, string? id, DateTime? startDate, DateTime? endDate, DateTime? expiredDate,
                    decimal? totalPrice, string? status, string? subscriptionId, string? userId);

		Task<Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total)>> GetAllSubscriptionPurchaseUser(string userId, int page, int pageSize);
    }
}
