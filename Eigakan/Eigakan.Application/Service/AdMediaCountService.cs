using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdMedia;
using Eigakan.Domain.Response.AdMediaCount;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class AdMediaCountService : IAdMediaCountService
	{
		private readonly IAdMediaCountRepository _adMediaCountRepository;
		private readonly IAdMediaRepository _adMediaRepository;
        private readonly IAdPurchaseItemRepository _adPurchaseItemRepository;
        private readonly ILogger<AdMediaCountService> _logger;

		public AdMediaCountService(
			IAdMediaCountRepository adMediaCountRepository,
			IAdMediaRepository adMediaRepository,
            IAdPurchaseItemRepository adPurchaseItemRepository,
            ILogger<AdMediaCountService> logger)
		{
			_adMediaCountRepository = adMediaCountRepository;
			_adMediaRepository = adMediaRepository;
            _adPurchaseItemRepository = adPurchaseItemRepository;
            _logger = logger;
		}

		public async Task<Result<AdMediaCount>> GetAdMediaCountByAdMediaId(string? adMediaId)
		{
			try
			{
				var adMediaCount = await _adMediaCountRepository.GetAdMediaCountByAdMediaId(adMediaId);
				if (adMediaCount == null)
				{
					return new Result<AdMediaCount>
					{
						Success = false,
						Message = "AdMediaCount not found"
					};
				}

				return new Result<AdMediaCount>
				{
					Success = true,
					Data = adMediaCount
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting ad media count");
				return new Result<AdMediaCount>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<Result<AdMediaCount>> IncreaseAdMediaCount(AdClickCountCreateRequest adClickCount)
		{
			try
			{
				if (string.IsNullOrEmpty(adClickCount.AdMediaId))
				{
					return new Result<AdMediaCount>
					{
						Success = false,
						Message = "AdMediaId is required"
					};
				}

				if (string.IsNullOrEmpty(adClickCount.MovieId))
				{
					return new Result<AdMediaCount>
					{
						Success = false,
						Message = "MovieId is required"
					};
				}

				// Check if AdMedia exists
				var adMedia = await _adMediaRepository.GetAdMediaById(adClickCount.AdMediaId);
				if (adMedia == null)
				{
					return new Result<AdMediaCount>
					{
						Success = false,
						Message = $"AdMedia with ID {adClickCount.AdMediaId} not found"
					};
				}

				var timeUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
				var jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
				var timeJst = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, jstZone);
				var dateOnly = DateOnly.FromDateTime(timeJst);

				var existingCount = await _adMediaCountRepository.CheckCountByAdMediaDate(adClickCount.AdMediaId, adClickCount.MovieId, dateOnly);

				if (existingCount != null)
				{
					existingCount.ViewCount++;
					var updatedCount = await _adMediaCountRepository.UpdateViewCount(adClickCount.AdMediaId, adClickCount.MovieId, dateOnly);
					if (updatedCount == null)
					{
						return new Result<AdMediaCount>
						{
							Success = false,
							Message = "Failed to update view count"
						};
					}

					return new Result<AdMediaCount>
					{
						Success = true,
						Data = updatedCount
					};
				}

				var newCount = new AdMediaCount
				{
					Id = Guid.NewGuid().ToString(),
					ViewCount = 1,
					ViewDate = dateOnly
				};

				await _adMediaCountRepository.InsertWithAdMedia(newCount, adClickCount.AdMediaId, adClickCount.MovieId);

				return new Result<AdMediaCount>
				{
					Success = true,
					Data = newCount
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error increasing ad media count");
				return new Result<AdMediaCount>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<object> StatisticAdMediaCount(string adMediaId)
		{
			try
			{
				var adMediaCounts = await _adMediaCountRepository.GetAllAdMediaCountByAdMediaId(adMediaId);

				if (!adMediaCounts.Any())
				{
					return new
					{
						Success = false,
						Message = "No statistics found"
					};
				}

				var statistics = adMediaCounts
					.GroupBy(x => x.ViewDate)
					.Select(g => new
					{
						ViewDate = g.Key,
						TotalViews = g.Sum(x => x.ViewCount)
					})
					.OrderByDescending(x => x.ViewDate)
					.ToList();

				return new
				{
					Success = true,
					Data = statistics
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting statistics");
				return new
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

        public async Task<Result<AdMediaCountGetAllResponse>> CreateCountAdMediaAsync(string mediaId)
        {
            var today = DateTime.UtcNow.AddHours(7);
            var todayDateOnly = DateOnly.FromDateTime(today);

            AdMediaCount todayCount = null;
            string message = string.Empty;

            try
            {

				// Kiểm tra AdPurchaseItem còn lượt hay không
				var purchaseItem = await _adPurchaseItemRepository.GetByMediaIdAndHasRemainingViews(mediaId);
				if (purchaseItem == null || (purchaseItem.RemainingViews ?? 0) <= 0)
				{
					return new Result<AdMediaCountGetAllResponse>
					{
						Success = false,
						Message = "No remaining views available for this AdMedia.",

					};
				}

				// Lấy hoặc tạo mới AdMediaCount
				todayCount = await _adMediaCountRepository.GetByMediaIdAndDate(mediaId, todayDateOnly);

                if (todayCount != null)
                {
                    todayCount.ViewCount = (todayCount.ViewCount ?? 0) + 1;
                    await _adMediaCountRepository.Update(todayCount);
                    message = "AdMediaCount updated successfully.";
                }
                else
                {
                    var newCount = new AdMediaCount
                    {
                        Id = Guid.NewGuid().ToString(),
                        AdMediaId = mediaId,
                        ViewDate = todayDateOnly,
                        ViewCount = 1
                    };
                    await _adMediaCountRepository.Insert(newCount);
                    todayCount = newCount;
                    message = "AdMediaCount created successfully.";
                }

                

                // Trừ lượt và cập nhật
                purchaseItem.RemainingViews--;

				purchaseItem.ConsumedViewFee = (purchaseItem.ConsumedViewFee ?? 0) + purchaseItem.PricePerView;


				if (purchaseItem.RemainingViews <= 0)
				{
					purchaseItem.Status = "INACTIVE";
					var adMedia = await _adMediaRepository.GetAdMediaById(mediaId);

					adMedia.status = "INACTIVE";
					await _adMediaRepository.Update(adMedia);
	
					
				}				
                await _adPurchaseItemRepository.Update(purchaseItem);


                // Trả response thành công
                var response = new AdMediaCountGetAllResponse
                {
                    Id = todayCount.Id,
                    AdMediaId = todayCount.AdMediaId,
                    ViewDate = todayCount.ViewDate,
                    ViewCount = todayCount.ViewCount
                };

                return new Result<AdMediaCountGetAllResponse>
                {
                    Success = true,
                    Message = message,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new Result<AdMediaCountGetAllResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

    }
} 