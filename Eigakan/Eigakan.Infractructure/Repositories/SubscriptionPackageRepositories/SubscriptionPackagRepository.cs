using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.SubscriptionPackageRepositories
{
    public class SubscriptionPackagRepository : GenericBase<SubscriptionPackage>, ISubscriptionPackageRepository
    {
        public async Task<List<SubscriptionPackage>> GetAllSubscriptionPackage(int page, int pageSize)
        {
            var subscriptionPackages = await Get(pageIndex: page,
                pageSize: pageSize);
            return subscriptionPackages.ToList();
        }
        public async Task<int> CountAllSubscriptionPackageAsync()
        {
            return await CountAsync();
        }
        public async Task<SubscriptionPackage> GetSubscriptionPackageById(string id)
        {
            return await GetSingle(
       filter: c => c.Id == id
   );
        }

    }
}
