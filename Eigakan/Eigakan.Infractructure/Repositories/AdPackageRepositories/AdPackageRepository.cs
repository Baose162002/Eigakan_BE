using Eigakan.Application.Interface.IRepository;

using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.AdPackageRepositories
{
    public class AdPackageRepository :GenericBase<AdPackage>, IAdPackageRepository
    {
		public async Task<List<AdPackage>> GetAllAdPackageAsync(int page, int pageSize)
		{
			return (await Get(
				orderBy: q => q
					.OrderByDescending(u => u.Status == "Active") 
					.ThenByDescending(u => u.CreateDate),         
				pageIndex: page,
				pageSize: pageSize
			))
			.ToList();
		}


		public async Task<int> CountAllAdPackageAsync()
		{
			return await CountAsync();
		}

		public async Task<AdPackage> GetAdPackageById(string id)
		{
			return await GetSingle(u => u.Id.Equals(id));
		}

		public async Task<List<AdPackage>> GetAdPackageByMinMax(int? min, int? max)
		{
			return (await Get(u =>
					u.Status == "Active" &&
					u.MinView <= max && u.MaxView >= min
			)).ToList();
		}


        public async Task<AdPackage?> GetFirstAdPackageByViewQuantityAsync(int viewQuantity)
        {
            return await GetSingle(p =>
				p.Status == "Active" &&
                p.MinView <= viewQuantity && viewQuantity <= p.MaxView);
        }

    }
}
