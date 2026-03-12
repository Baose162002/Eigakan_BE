using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdPackage;
using Eigakan.Domain.Request.AdSlotTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IAdPackageService
    {
        Task<Result<AdPackage>> CreateAdPackage(AdPackageCreateRequest request);
        Task<Result<AdPackage>> GetAdPackageById(string? id);
        Task<Result<AdPackage>> UpdateAdPackage(string? id, AdPackageUpdateRequest request);
        Task<(List<AdPackage> AdPackages, int Total)> GetAllAdPackageAsync(int page, int pageSize);
        Task<Result<AdPackage>> GetAdPackageByQuantity(int quantity);

	}
}
