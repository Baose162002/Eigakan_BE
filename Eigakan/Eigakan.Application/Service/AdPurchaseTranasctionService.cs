using AutoMapper;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdPurchaseItem;
using Eigakan.Domain.Response.AdPurchaseTransaction;
using Eigakan.Domain.Response.ContractResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class AdPurchaseTranasctionService : IAdPurchaseTransactionService
    {
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IAdPackageRepository _adPackageRepository;
        private readonly IAdMediaRepository _adMediaRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdPurchaseItemRepository _adPurchaseItemRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IAdPurchaseTransactionRepository _adPurchaseTransactionRepository;
        private readonly IMapper _mapper;

        public AdPurchaseTranasctionService(IUserWalletRepository userWalletRepository, IAdPackageRepository adPackageRepository, IAdMediaRepository adMediaRepository, IHttpContextAccessor httpContextAccessor, IAdPurchaseItemRepository adPurchaseItemRepository, IWalletTransactionRepository walletTransactionRepository, IAdPurchaseTransactionRepository adPurchaseTransactionRepository, IMapper mapper)
        {
            _userWalletRepository = userWalletRepository;
            _adPackageRepository = adPackageRepository;
            _adMediaRepository = adMediaRepository;
            _httpContextAccessor = httpContextAccessor;
            _adPurchaseItemRepository = adPurchaseItemRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _adPurchaseTransactionRepository = adPurchaseTransactionRepository;
            _mapper = mapper;
        }
        public async Task<Result<AdPurchaseTransactionGetAllResponse>> CreateAdPurchaseAsync(CreateAdPurchaseRequest request)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;

            var userWallet = await _userWalletRepository.GetUserWalletById(userId);
            if (userWallet == null)
                return new Result<AdPurchaseTransactionGetAllResponse>
                {
                    Success = false,
                    Message = "Wallet not found"
                };

            decimal estimatedTotalPrice = 0;

            // Step 1: Tính trước tổng giá tiền các AdPurchaseItems
            foreach (var item in request.AdPurchaseItems)
            {
                var package = await _adPackageRepository.GetFirstAdPackageByViewQuantityAsync(item.ViewQuantity);
                if (package == null)
                {
                    return new Result<AdPurchaseTransactionGetAllResponse>
                    {
                        Success = false,
                        Message = $"No AdPackage available for {item.ViewQuantity} views"
                    };
                }

                estimatedTotalPrice += (package.PricePerView ?? 0) * item.ViewQuantity;
            }

            if ((userWallet.Balance ?? 0) < estimatedTotalPrice)
            {
                return new Result<AdPurchaseTransactionGetAllResponse>
                {
                    Success = false,
                    Message = "Insufficient balance"
                };
            }

            decimal totalPrice = 0;
            var adPurchaseItems = new List<AdPurchaseItems>();

            await using var transaction = await _adPurchaseTransactionRepository.BeginTransactionAsync();
            try
            {
                foreach (var item in request.AdPurchaseItems)
                {
                    var package = await _adPackageRepository.GetFirstAdPackageByViewQuantityAsync(item.ViewQuantity);
                    AdMedia media;

                    if (!string.IsNullOrEmpty(item.MediaId) && item.NewMedia != null)
                    {
                        return new Result<AdPurchaseTransactionGetAllResponse>
                        {
                            Success = false,
                            Message = "You must provide either an existing MediaId or NewMedia"
                        };
                    }

                    if (!string.IsNullOrEmpty(item.MediaId))
                    {
                        var existingMedia = await _adMediaRepository.GetAdMediaById(item.MediaId);
                        if (existingMedia == null)
                            return new Result<AdPurchaseTransactionGetAllResponse>
                            {
                                Success = false,
                                Message = $"Media with ID '{item.MediaId}' not found"
                            };

                        if (existingMedia.status == "ACTIVE" || existingMedia.status == "PENDING")
                            return new Result<AdPurchaseTransactionGetAllResponse>
                            {
                                Success = false,
                                Message = "Cannot reuse media that is already ACTIVE"
                            };

                        media = new AdMedia
                        {
                            Id = Guid.NewGuid().ToString(),
                            Content = existingMedia.Content,
                            Url = existingMedia.Url,
                            CreateAt = DateTime.UtcNow.AddHours(7),
                            status = "PENDING"
                        };
                        await _adMediaRepository.InsertTransaction(media);
                    }
                    else if (item.NewMedia != null)
                    {
                        media = new AdMedia
                        {
                            Id = Guid.NewGuid().ToString(),
                            Content = item.NewMedia.Content,
                            Url = item.NewMedia.Url,
                            CreateAt = DateTime.UtcNow.AddHours(7),
                            status = "PENDING"
                        };
                        await _adMediaRepository.InsertTransaction(media);
                    }
                    else
                    {
                        return new Result<AdPurchaseTransactionGetAllResponse>
                        {
                            Success = false,
                            Message = "You must provide either an existing MediaId or NewMedia"
                        };
                    }

                    var price = (package.PricePerView ?? 0) * item.ViewQuantity;

                    adPurchaseItems.Add(new AdPurchaseItems
                    {
                        Id = Guid.NewGuid().ToString(),
                        ViewQuantity = item.ViewQuantity,
                        PricePerView = package.PricePerView,
                        Price = price,
                        RemainingViews = item.ViewQuantity,
                        ExpiredDate = DateTime.UtcNow.AddHours(7).AddMonths(3),
                        CreatedDate = DateTime.UtcNow.AddHours(7),
                        AdMediaId = media.Id,
                        AdPackageId = package.Id,
                        Status = "PENDING",
                    });

                    totalPrice += price;
                }

                var adTransaction = new AdPurchaseTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    TotalPrice = totalPrice,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    Status = "SUCCESS",
                    UserId = userId,
                    AdPurchaseItems = adPurchaseItems
                };
                await _adPurchaseTransactionRepository.InsertTransaction(adTransaction);

                userWallet.Balance -= totalPrice;
                await _userWalletRepository.UpdateTransaction(userWallet);

                await _walletTransactionRepository.InsertTransaction(new WalletTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = totalPrice,
                    Type = "AD_PURCHASE",
                    PaymentReferenceId = adTransaction.Id,
                    PaymentMethod = "WALLET",
                    Status = "SUCCESS",
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    UserWalletId = userWallet.Id
                });
                await _adMediaRepository.SaveChangeTransaction();
                await _adPurchaseTransactionRepository.SaveChangeTransaction();
                await _userWalletRepository.SaveChangeTransaction();
                await _walletTransactionRepository.SaveChangeTransaction();
                await transaction.CommitAsync();

                return new Result<AdPurchaseTransactionGetAllResponse>
                {
                    Success = true,
                    Message = "Purchase successful",
                    Data = _mapper.Map<AdPurchaseTransactionGetAllResponse>(adTransaction)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Result<AdPurchaseTransactionGetAllResponse>
                {
                    Success = false,
                    Message = $"Transaction failed: {ex.Message}"
                };
            }
        }

        public async Task<Result<(List<AdPurchaseTransactionGetAllResponse> Data, int Total)>> GetListAdPurchaseTransactionForUser(int page, int pageSize)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new Result<(List<AdPurchaseTransactionGetAllResponse>, int)>
                    {
                        Success = false,
                        Message = "User not authenticated."
                    };
                }

                var adPurchaseTransactions = await _adPurchaseTransactionRepository.GetAdPurchaseTransactionByUserIdPaging(userId, page, pageSize);
                var total = await _adPurchaseTransactionRepository.CountAllAdPuchaseTransactionByUserIdAsync(userId);

                return new Result<(List<AdPurchaseTransactionGetAllResponse>, int)>
                {
                    Success = true,
                    Data = (_mapper.Map<List<AdPurchaseTransactionGetAllResponse>>(adPurchaseTransactions), total)
                };
            }
            catch (Exception ex)
            {
                return new Result<(List<AdPurchaseTransactionGetAllResponse>, int)>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<Result<(List<AdPurchaseTransactionGetAllResponse> Data, int Total)>> GetListAllAdPurchaseTransaction(int page, int pageSize)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new Result<(List<AdPurchaseTransactionGetAllResponse>, int)>
                    {
                        Success = false,
                        Message = "User not authenticated."
                    };
                }

                var adPurchaseTransactions = await _adPurchaseTransactionRepository.GetAllAdPurchaseTransaction(page, pageSize);
                var total = await _adPurchaseTransactionRepository.CountAllAdPuchaseTransactionAsync();

                return new Result<(List<AdPurchaseTransactionGetAllResponse>, int)>
                {
                    Success = true,
                    Data = (_mapper.Map<List<AdPurchaseTransactionGetAllResponse>>(adPurchaseTransactions), total)
                };
            }
            catch (Exception ex)
            {
                return new Result<(List<AdPurchaseTransactionGetAllResponse>, int)>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

    }
}
