using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface ISubscriptionPackageRepository : IGenericRepository<SubscriptionPackage>
    {
        Task<List<SubscriptionPackage>> GetAllSubscriptionPackage(int page, int pageSize);
        Task<SubscriptionPackage> GetSubscriptionPackageById(string id);
        Task<int> CountAllSubscriptionPackageAsync();
    }
}
