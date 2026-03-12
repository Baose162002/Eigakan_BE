using AutoMapper;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdMedia;
using Eigakan.Domain.Response.AdMediaCount;
using Eigakan.Domain.Response.AdMediaResponse;
using Eigakan.Domain.Response.AdPurchaseItem;
using Eigakan.Domain.Response.UserWallet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class AdMediaService : IAdMediaService
    {
        private readonly IAdMediaRepository _adMediaRepository;
        private readonly Logger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdMediaCountRepository _adMediaCountRepository;
        private readonly IAdPurchaseItemRepository _adPurchaseItemRepository;
        private readonly IAdPackageRepository _adPackageRepository;
        private readonly IAdPurchaseTransactionRepository _adPurchaseTransactionRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IMoviesRepository _moviesRepository;
        private readonly IMapper _mapper;
        public AdMediaService(IAdMediaRepository adMediaRepository, Logger logger, IAdMediaCountRepository adMediaCountRepository, IAdPurchaseTransactionRepository adPurchaseTransactionRepository,
        IAdPurchaseItemRepository adPurchaseItemRepository, IMoviesRepository moviesRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAdPackageRepository adPackageRepository, IUserWalletRepository userWalletRepository, IWalletTransactionRepository walletTransactionRepository)
        {
            _adMediaRepository = adMediaRepository;
            _logger = logger;
            _adMediaCountRepository = adMediaCountRepository;
            _adPurchaseItemRepository = adPurchaseItemRepository;
            _moviesRepository = moviesRepository;
            _adPurchaseTransactionRepository = adPurchaseTransactionRepository;
            _userWalletRepository = userWalletRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _adPackageRepository = adPackageRepository;
            _walletTransactionRepository = walletTransactionRepository;
        }

        public async Task<Result<List<AdMediaWithPositionDto>>> GetListMediaActive()
        {
            var transactions = await _adMediaRepository.GetListMediaActive();

            return new Result<List<AdMediaWithPositionDto>>
            {
                Success = true,
                Message = "Success",
                Data = _mapper.Map<List<AdMediaWithPositionDto>>(transactions)
            };
        }

        public async Task<List<AdMediaWithPositionDto>> GetAdMediaWithPositionsAsync(string movieId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var mediaWithPositions = new List<AdMediaWithPositionDto>();

            try
            {
                // 1. Lấy thông tin movie
                var movie = await _moviesRepository.GetMovieById(movieId);
                if (movie == null || !movie.Duration.HasValue)
                    return mediaWithPositions;

                TimeSpan movieDuration = TimeSpan.FromMinutes(movie.Duration.Value);
                double totalSeconds = movieDuration.TotalSeconds;

                if (totalSeconds < 120)
                    return mediaWithPositions; // Không chèn quảng cáo với phim < 2 phút

                // 2. Lấy danh sách media hợp lệ
                var allMedia = await _adMediaRepository.GetListMediaActive();
                var scoredMedia = new List<(AdMedia media, int score)>();
                var neverShownMedia = new List<AdMedia>();
                var candidateForOldMedia = new List<AdMedia>();

                foreach (var media in allMedia)
                {
                    var purchaseItem = await _adPurchaseItemRepository.GetByMediaIdAndHasRemainingViews(media.Id);
                    if (purchaseItem == null) continue;

                    var todayCount = await _adMediaCountRepository.GetByMediaIdAndDate(media.Id, today);
                    int todayViewCount = todayCount?.ViewCount ?? 0;

                    // Tính score
                    int score = (purchaseItem.RemainingViews ?? 0) - todayViewCount;

                    if (score <= 0) continue;

                    // Kiểm tra media chưa từng hiển thị (không có bất kỳ AdMediaCount nào)
                    bool hasAnyCount = await _adMediaCountRepository.HasAnyViewCount(media.Id);
                    if (!hasAnyCount)
                    {
                        neverShownMedia.Add(media);
                    }
                    else
                    {
                        // Kiểm tra media đã 3 ngày không lên sóng
                        var lastViewDate = await _adMediaCountRepository.GetLastViewDate(media.Id);
                        if (lastViewDate.HasValue && (today.DayNumber - lastViewDate.Value.DayNumber) >= 3)
                        {
                            candidateForOldMedia.Add(media);
                        }

                    }

                    scoredMedia.Add((media, score));
                }

                // Ưu tiên media chưa từng lên sóng
                if (neverShownMedia.Any())
                {
                    var prioritized = neverShownMedia.Select(m => (m, score: int.MaxValue));
                    scoredMedia = prioritized.Concat(scoredMedia.Where(x => !neverShownMedia.Contains(x.media))).ToList();
                }

                var topMedia = scoredMedia.OrderByDescending(x => x.score).ToList();
                if (topMedia.Count == 0) return mediaWithPositions;

                Random rand = new Random();
                var adPositions = new List<int>();
                int totalAdSlots = 0;

                if (totalSeconds >= 600) // Phim từ 10 phút trở lên
                {
                    double maxAdTime = totalSeconds - 600; // Trừ 10 phút cuối
                    totalAdSlots = (int)(movieDuration.TotalMinutes / 10);
                    totalAdSlots = Math.Min(totalAdSlots, topMedia.Count);

                    double interval = maxAdTime / totalAdSlots;
                    adPositions.Add(5); // Đầu tiên cố định ở giây thứ 5

                    for (int i = 1; i < totalAdSlots; i++)
                    {
                        double basePosition = interval * i;
                        int offset = rand.Next(-30, 31);
                        int position = (int)Math.Clamp(basePosition + offset, 6, maxAdTime);

                        if (adPositions.All(p => Math.Abs(p - position) >= 120))
                        {
                            adPositions.Add(position);
                        }
                    }
                }
                else // Phim từ 2 đến dưới 10 phút
                {
                    totalAdSlots = Math.Min(2, topMedia.Count); // Tối đa 2 quảng cáo
                    adPositions.Add(5); // Đầu tiên ở giây thứ 5

                    if (totalAdSlots > 1)
                    {
                        int minDistance = 90;
                        int secondSlot = (int)Math.Clamp(totalSeconds - 10, adPositions[0] + minDistance, totalSeconds - 1);
                        if (secondSlot - adPositions[0] >= minDistance)
                            adPositions.Add(secondSlot);
                    }
                }

                adPositions.Sort();

                // Slot cuối sẽ dành cho media đã 3 ngày không xuất hiện
                int lastSlot = adPositions.LastOrDefault();
                AdMedia? selectedOldMedia = null;

                if (candidateForOldMedia.Any())
                {
                    selectedOldMedia = candidateForOldMedia[rand.Next(candidateForOldMedia.Count)];
                    adPositions.Remove(lastSlot);

                    mediaWithPositions.Add(new AdMediaWithPositionDto
                    {
                        AdMediaId = selectedOldMedia.Id,
                        Position = lastSlot,
                        AdMedia = new AdMediaGetAll
                        {
                            Id = selectedOldMedia.Id,
                            Content = selectedOldMedia.Content,
                            Url = selectedOldMedia.Url,
                            ReasonForRejection = selectedOldMedia.ReasonForRejection,
                            status = selectedOldMedia.status,
                            ApprovedDate = selectedOldMedia.ApprovedDate,
                            CreateAt = selectedOldMedia.CreateAt
                        }
                    });

                    // Loại khỏi topMedia để không lặp lại
                    topMedia = topMedia.Where(x => x.media.Id != selectedOldMedia.Id).ToList();
                }

                // Lấy số lượng slot
                var mediaToAssign = topMedia
                    .Where(x => selectedOldMedia == null || x.media.Id != selectedOldMedia.Id)
                    .Take(adPositions.Count)
                    .Select(x => x.media)
                    .ToList();

                // Shuffle media list
                mediaToAssign = mediaToAssign.OrderBy(_ => rand.Next()).ToList();

                // Shuffle positions list (optionally)
                adPositions = adPositions.OrderBy(_ => rand.Next()).ToList();

                // Gán media vào random vị trí
                for (int i = 0; i < mediaToAssign.Count && i < adPositions.Count; i++)
                {
                    var media = mediaToAssign[i];
                    var position = adPositions[i];

                    mediaWithPositions.Add(new AdMediaWithPositionDto
                    {
                        AdMediaId = media.Id,
                        Position = position,
                        AdMedia = new AdMediaGetAll
                        {
                            Id = media.Id,
                            Content = media.Content,
                            Url = media.Url,
                            ReasonForRejection = media.ReasonForRejection,
                            status = media.status,
                            ApprovedDate = media.ApprovedDate,
                            CreateAt = media.CreateAt
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching AdMedia with positions.");
            }

            return mediaWithPositions;
        }

        public async Task<Result<List<AdMediaGetAllResponse>>> GetMediaByUserIdAsync(int page, int pageSize)
        {
            var response = new List<AdMediaGetAllResponse>();
            var message = "";
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = false,
                    Message = "User not authenticated"
                };
            }
            // 1. Get all ad transactions by user
            var transactions = await _adPurchaseTransactionRepository.GetAdPurchaseTransactionByUserId(userId);
            if (transactions == null || !transactions.Any())
            {
                message = "No ad transactions found.";
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = true,
                    Message = message,
                    Data = response
                };
            }

            // 2. Get all AdPurchaseItems from each transaction
            var allItems = new List<AdPurchaseItems>();
            foreach (var transaction in transactions)
            {
                var items = await _adPurchaseItemRepository.GetItemsByTransactionIdAsync(transaction.Id);
                allItems.AddRange(items);
            }

            if (!allItems.Any())
            {
                message = "No ad items found.";
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = true,
                    Message = message,
                    Data = response
                };
            }

            // 3. Get all distinct media IDs
            var mediaIds = allItems
                .Select(i => i.AdMediaId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (!mediaIds.Any())
            {
                message = "No media found.";
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = true,
                    Message = message,
                    Data = response
                };
            }

            // 4. Pagination
            var pagedMediaIds = mediaIds
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mediaList = new List<AdMedia>();
            foreach (var mediaId in pagedMediaIds)
            {
                var medias = await _adMediaRepository.GetListMediaByUserId(mediaId, 1, 1);
                mediaList.AddRange(medias);
            }

            // 5. Map to DTO
            response = mediaList
                .OrderByDescending(m => m.CreateAt)
                .Select(m => new AdMediaGetAllResponse
                {
                    Id = m.Id,
                    Content = m.Content,
                    Url = m.Url,
                    ReasonForRejection = m.ReasonForRejection,
                    status = m.status,
                    ApprovedDate = m.ApprovedDate,
                    CreateAt = m.CreateAt
                })
                .ToList();

            return new Result<List<AdMediaGetAllResponse>>
            {
                Success = true,
                Message = "Media list retrieved successfully.",
                Data = response
            };
        }
        
        public async Task<Result<List<AdMediaGetAllResponse>>> GetMediaStatusEXpiredByUserIdAsync(int page, int pageSize)
        {
            var response = new List<AdMediaGetAllResponse>();
            var message = "";
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = false,
                    Message = "User not authenticated"
                };
            }
            // 1. Get all ad transactions by user
            var transactions = await _adPurchaseTransactionRepository.GetAdPurchaseTransactionByUserId(userId);
            if (transactions == null || !transactions.Any())
            {
                message = "No ad transactions found.";
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = true,
                    Message = message,
                    Data = response
                };
            }

            // 2. Get all AdPurchaseItems from each transaction
            var allItems = new List<AdPurchaseItems>();
            foreach (var transaction in transactions)
            {
                var items = await _adPurchaseItemRepository.GetItemsByTransactionIdAsync(transaction.Id);
                allItems.AddRange(items);
            }

            if (!allItems.Any())
            {
                message = "No ad items found.";
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = true,
                    Message = message,
                    Data = response
                };
            }

            // 3. Get all distinct media IDs
            var mediaIds = allItems
                .Select(i => i.AdMediaId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (!mediaIds.Any())
            {
                message = "No media found.";
                return new Result<List<AdMediaGetAllResponse>>
                {
                    Success = true,
                    Message = message,
                    Data = response
                };
            }

            // 4. Pagination
            var pagedMediaIds = mediaIds
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mediaList = new List<AdMedia>();
            foreach (var mediaId in pagedMediaIds)
            {
                var medias = await _adMediaRepository.GetListMediaStatusExpiredByUserId(mediaId, 1, 1);
                mediaList.AddRange(medias);
            }

            // 5. Map to DTO
            response = mediaList
                .OrderByDescending(m => m.CreateAt)
                .Select(m => new AdMediaGetAllResponse
                {
                    Id = m.Id,
                    Content = m.Content,
                    Url = m.Url,
                    ReasonForRejection = m.ReasonForRejection,
                    status = m.status,
                    ApprovedDate = m.ApprovedDate,
                    CreateAt = m.CreateAt
                })
                .ToList();

            return new Result<List<AdMediaGetAllResponse>>
            {
                Success = true,
                Message = "Media list retrieved successfully.",
                Data = response
            };
        }
        
        public async Task<Result<List<AdMedia>>> GetAllListAdMedia(string? status, int page, int pageSize)
        {
            try
            {
                var adMedia = await _adMediaRepository.GetList(status, page, pageSize);
                return new Result<List<AdMedia>>
                {
                    Success = true,
                    Data = adMedia,
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(AdMediaService));
                return new Result<List<AdMedia>> { Success = false, Message = ex.Message };
            }
        }

		
        
        public async Task<Result<List<AdMedia>>> GetAllListAdMediaActive()
		{
			try
			{
                var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                var start = new DateTime(0, 0, 0,6,0,0);  // 6:00 AM
                var end = new TimeSpan(18, 0, 0,18,0,0);   // 6:00 PM
                var adMedia = await _adMediaRepository.GetListActive();

                return new Result<List<AdMedia>>
				{
					Success = true,
					Data = adMedia,
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(AdMediaService));
				return new Result<List<AdMedia>> { Success = false, Message = ex.Message };
			}
		}

        //public async Task<Result<List<AdMedia>>> GetListActiveFollowTime()
        //{
        //    try
        //    {         
        //        var adMedia = await _adMediaRepository.GetListActiveFollowTime();

        //        return new Result<List<AdMedia>>
        //        {
        //            Success = true,
        //            Data = adMedia,
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logger.LogError(ex, nameof(AdMediaService));
        //        return new Result<List<AdMedia>> { Success = false, Message = ex.Message };
        //    }
        //}
        
        public async Task<Result<AdMedia>> GetById(string? id)
        {
            try
            {
                var adMedia = await _adMediaRepository.GetAdMediaById(id);

                if (adMedia == null)
                {
                    return new Result<AdMedia>
                    {
                        Success = false,
                        Message = "AdMedia not found.",

                    };
                }

                return new Result<AdMedia>
                {
                    Success = true,
                    Message = "Success.",
                    Data = adMedia,
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(AdMediaService));
                return new Result<AdMedia> { Success = false, Message = ex.Message };
            }
        }




        
        
        public async Task<Result<AdMedia>> AdMediaApprovedStatus(AdMediaApprovedStatus request)
        {
            try
            {
                var adMedia = await _adMediaRepository.GetAdMediaById(request.Id);

                if (adMedia == null)
                {
                    return new Result<AdMedia>
                    {
                        Success = false,
                        Message = "AdMedia not found.",
                    };
                }

                adMedia.ApprovedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                adMedia.status = AdStatusEnum.ACTIVE.ToString();
                adMedia.ReasonForRejection = null;

                await _adMediaRepository.UpdateTransaction(adMedia);

                // Cập nhật AdPurchaseItem liên quan
                var adPurchaseItem = await _adPurchaseItemRepository.GetByMediaIdAndHasRemainingViews(request.Id);
                if (adPurchaseItem != null)
                {
                    adPurchaseItem.Status = "ACTIVE"; 
                    await _adPurchaseItemRepository.UpdateTransaction(adPurchaseItem);
                }

                await _adMediaRepository.SaveChangeTransaction();
                await _adPurchaseItemRepository.SaveChangeTransaction();

                return new Result<AdMedia>
                {
                    Success = true,
                    Data = adMedia,
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(AdMediaService));
                return new Result<AdMedia> { Success = false, Message = ex.Message };
            }
        }

        
        
        public async Task<Result<AdMedia>> AdMediaRejectedStatus(AdMediaRejectedRequest request)
        {
            IDbContextTransaction transaction = null;

            try
            {
                var adMedia = await _adMediaRepository.GetAdMediaById(request.Id);
                if (adMedia == null)
                {
                    return new Result<AdMedia> { Success = false, Message = "AdMedia not found." };
                }

                adMedia.ApprovedDate = null;
                adMedia.status = AdStatusEnum.REJECTED.ToString();
                adMedia.ReasonForRejection = request.ReasonForRejection;

                await _adMediaRepository.UpdateTransaction(adMedia);
                await _adMediaRepository.SaveChangeTransaction();

                var adPurchaseItem = await _adPurchaseItemRepository.GetByMediaIdAndHasRemainingViews(adMedia.Id);
                transaction = await _adPurchaseTransactionRepository.BeginTransactionAsync();
                await using (transaction)
                {
                    if (adPurchaseItem != null)
                    {
                        var adPurchaseTransaction = await _adPurchaseTransactionRepository.GetAdPurchaseTransactionById(adPurchaseItem.AdPurchaseTransactionId);
                        if (adPurchaseTransaction != null)
                        {
                            var refundAmount = adPurchaseItem.RemainingViews * adPurchaseItem.PricePerView;
                            var userWallet = await _userWalletRepository.GetUserWalletById(adPurchaseTransaction.UserId);
                            if (userWallet != null)
                            {
                                var adPackage = await _adPackageRepository.GetAdPackageById(adPurchaseItem.AdPackageId);
                                var packageName = adPackage?.PackageName ?? $"PackageId {adPurchaseItem.AdPackageId}";

                                userWallet.Balance ??= 0;
                                userWallet.Balance += refundAmount;

                                var refundTransaction = new WalletTransaction
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    UserWalletId = userWallet.Id,
                                    Amount = refundAmount,
                                    Type = $"Refund AdPackage \"{packageName}\"",
                                    PaymentMethod = "SYSTEM",
                                    PaymentReferenceId = adPurchaseItem.Id,
                                    Status = "SUCCESS",
                                    CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                                };

                                await _userWalletRepository.UpdateTransaction(userWallet);
                                await _walletTransactionRepository.InsertTransaction(refundTransaction);
                               
                                adPurchaseItem.RefundedPrice = refundAmount;
                                adPurchaseItem.Status = "REFUNDED";

                                await _adPurchaseItemRepository.UpdateTransaction(adPurchaseItem);

                                await _userWalletRepository.SaveChangeTransaction();
                                await _walletTransactionRepository.SaveChangeTransaction();
                                await _adPurchaseItemRepository.SaveChangeTransaction();

                                await transaction.CommitAsync();
                            }
                        }
                    }
                }

                return new Result<AdMedia> { Success = true, Data = adMedia };
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                await _logger.LogError(ex, nameof(AdMediaService));
                return new Result<AdMedia> { Success = false, Message = ex.Message };
            }

        }   
        
        public async Task<Result<AdMedia>> Delete(string? id)
        {
            try
            {
                var media = await _adMediaRepository.GetAdMediaById(id);
                if (media == null)
                {
                    return new Result<AdMedia>
                    {
                        Success = false,
                        Message = "AdMedia not found.",

                    };
                }
                if (await _adMediaRepository.DeleteAdMediaAsync(media.Id))
                {

                    return new Result<AdMedia>
                    {
                        Success = true,
                        Message = "Delete success."
                    };
                }
                else
                {
                    return new Result<AdMedia>
                    {
                        Success = false,
                        Message = "Delete fail."
                    };
                }
            }
            catch (Exception ex)
            {
                //   await _logger.LogError(ex, nameof(AdMediaService));
                return new Result<AdMedia> { Success = false, Message = ex.Message };
            }
        }

	
    }
}
