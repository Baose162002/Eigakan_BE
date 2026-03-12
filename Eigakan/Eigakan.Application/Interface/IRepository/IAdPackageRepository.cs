using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IAdPackageRepository : IGenericRepository<AdPackage>
    {
		Task<List<AdPackage>> GetAllAdPackageAsync(int page, int pageSize);
		Task<int> CountAllAdPackageAsync();
		Task<AdPackage> GetAdPackageById(string id);
		Task<List<AdPackage>> GetAdPackageByMinMax(int? min, int? max);
        Task<AdPackage?> GetFirstAdPackageByViewQuantityAsync(int viewQuantity);

    }
}
