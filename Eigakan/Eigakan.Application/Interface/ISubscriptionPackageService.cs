using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Request.SubscriptionPackageRequest;
using Eigakan.Domain.Response.SubscriptionPackageResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface ISubscriptionPackageService
    {
        Task<Result<(List<SubscriptionPackageGetAllResponse> SubscriptionPackages, int Total)>> GetAllSubscriptionPackageAsync(int page, int pageSize);
        Task<Result<SubscriptionPackageGetAllResponse>> GetSubscriptionPackageById(string id);
        Task<Result<SubscriptionPackageGetAllResponse>> CreateSubscriptionPackageAsync(SubscriptionPackageCreateRequest request);
        Task<Result<SubscriptionPackageGetAllResponse>> UpdateSubscriptionPackageAsync(string subscriptionpackageId, SubscriptionPackageUpdateRequest request);
        Task<Result<SubscriptionPackageGetAllResponse>> UpdateSubscriptionPackageStatusAsync(string subscriptionpackageId);
    }
}
