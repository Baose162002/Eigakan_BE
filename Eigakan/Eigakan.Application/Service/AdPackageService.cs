using AutoMapper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdPackage;
using Eigakan.Domain.Request.AdSlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Eigakan.Application.Service
{
    public class AdPackageService : IAdPackageService
    {
        private readonly IAdPackageRepository _adPackageRepository;
        private readonly Logger _logger;
		private readonly IMapper _mapper;

		public AdPackageService(IAdPackageRepository adPackageRepository, Logger logger,
								IMapper mapper)
        {
            _adPackageRepository = adPackageRepository;
            _logger = logger;
			_mapper = mapper;
		}

		public async Task<(List<AdPackage> AdPackages, int Total)> GetAllAdPackageAsync(int page, int pageSize)
		{
			
			var listPackage = await _adPackageRepository.GetAllAdPackageAsync(page, pageSize);

			
			var total = await _adPackageRepository.CountAllAdPackageAsync();

			
			return (_mapper.Map<List<AdPackage>>(listPackage), total);
		}

		public async Task<Result<AdPackage>> GetAdPackageById(string? id)
		{
			var adPackage = await _adPackageRepository.GetAdPackageById(id);
			if (adPackage == null)
			{
				return new Result<AdPackage> { Success = false, Message = "Ad package not found" };
			}
			return new Result<AdPackage>
			{
				Success = true,
				Message = "Ad package retrieved successfully",
				Data = adPackage,
			};
		}

		public async Task<Result<AdPackage>> CreateAdPackage(AdPackageCreateRequest request)
		{
			try
			{
				if (request.MinView > request.MaxView)
				{
					return new Result<AdPackage> { Success = false, Message = "MinView cannot be greater than MaxView" };
				}

				var overlappingPackages = await _adPackageRepository.GetAdPackageByMinMax(request.MinView, request.MaxView);

				if (overlappingPackages.Any())
				{
					return new Result<AdPackage>
					{
						Success = false,
						Message = "There's already an active package that overlaps with this view range."
					};
				}

				var adPackage = new AdPackage
				{
					Id = Guid.NewGuid().ToString(),
					PackageName = request.PackageName,
					MinView = request.MinView,
					MaxView = request.MaxView,
					PricePerView = request.PricePerView,
					Status = "Active",
					CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
				};

				await _adPackageRepository.Insert(adPackage);
				
				return new Result<AdPackage>
				{
					Success = true,
					Message = "Ad package created successfully",
					Data = adPackage,
				};

			}catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(AdPackageService));
				return new Result<AdPackage> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<AdPackage>> UpdateAdPackage(string? id, AdPackageUpdateRequest request)
		{
			try
			{
				if (request.MinView > request.MaxView)
				{
					return new Result<AdPackage> { Success = false, Message = "MinView cannot be greater than MaxView" };
				}

				var adPackage = await _adPackageRepository.GetAdPackageById(id);
				if (adPackage == null)
				{
					return new Result<AdPackage> { Success = false, Message = "Ad package not found" };
				}


				var overlappingPackages = await _adPackageRepository.GetAdPackageByMinMax(request.MinView, request.MaxView);

				
				overlappingPackages = overlappingPackages
					.Where(p => p.Id != id)
					.ToList();

				if (overlappingPackages.Any())
				{
					return new Result<AdPackage>
					{
						Success = false,
						Message = "There's already an active package that overlaps with this view range."
					};
				}

				adPackage.PackageName = request.PackageName;
				adPackage.MinView = request.MinView;
				adPackage.MaxView = request.MaxView;
				adPackage.PricePerView = request.PricePerView;
				adPackage.Status = request.Status;

				await _adPackageRepository.Update(adPackage);

				return new Result<AdPackage>
				{
					Success = true,
					Message = "Ad package updated successfully",
					Data = adPackage,
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(AdPackageService));
				return new Result<AdPackage> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<AdPackage>> GetAdPackageByQuantity(int quantity)
		{
			try
			{
				var adPackage = await  _adPackageRepository.GetFirstAdPackageByViewQuantityAsync(quantity);
				if (adPackage == null)
				{
					return new Result<AdPackage> { Success = false, Message = "Ad package not found" };
				}

				return new Result<AdPackage>
				{
					Success = true,
					Message = "Get ad package successfully",
					Data = adPackage,
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(AdPackageService));
				return new Result<AdPackage> { Success = false, Message = ex.Message };
			}
		}

	}
}
