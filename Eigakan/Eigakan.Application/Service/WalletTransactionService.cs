using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdPurchaseTransaction;
using Eigakan.Domain.Request.VNPayRequest;
using Eigakan.Domain.Request.WalletTranasction;
using Eigakan.Domain.Response.AdPurchaseItem;
using Eigakan.Domain.Response.WalletTransaction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IVnPayService _vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IMapper _mapper;

        public WalletTransactionService(IVnPayService vnPayService, IHttpContextAccessor httpContextAccessor, IWalletTransactionRepository walletTransactionRepository, IUserWalletRepository userWalletRepository, IMapper mapper)
        {
            _vnPayService = vnPayService;
            _httpContextAccessor = httpContextAccessor;
            _walletTransactionRepository = walletTransactionRepository;
            _userWalletRepository = userWalletRepository;
            _mapper = mapper;
        }
        public async Task<Result<WalletTransaction>> CreatePayment(WalletTransactionCreateRequest request)
        {
            using var transaction = await _walletTransactionRepository.BeginTransactionAsync();

            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new Result<WalletTransaction>
                    {
                        Success = false,
                        Message = "User not authenticated.",
                    };
                }

                // 1. Kiểm tra hoặc tạo mới UserWallet
                var userWallet = await _userWalletRepository.GetUserWalletById(userId);
                if (userWallet == null)
                {
                    userWallet = new UserWallet
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        Balance = 0,
                        Status = "Active"
                    };

                    await _userWalletRepository.Insert(userWallet);
                    await _userWalletRepository.SaveChangeTransaction(); // bạn có thể gộp commit nếu cùng transaction
                }
                var now = DateTime.UtcNow;
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var currentTime = TimeZoneInfo.ConvertTimeFromUtc(now, vietnamTimeZone);
                // 2. Tạo WalletTransaction mới
                var walletTransaction = new WalletTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = request.Amount,
                    Type = "DEPOSIT",
                    PaymentReferenceId = Guid.NewGuid().ToString(),
                    PaymentMethod = "VNPay",
                    Status = "PENDING",
                    CreateDate = currentTime,
                    UserWalletId = userWallet.Id,
                };

                await _walletTransactionRepository.Insert(walletTransaction);
                await _walletTransactionRepository.SaveChangeTransaction();

                // 3. Tạo URL thanh toán
                var vnPayRequest = new VnPayRequest
                {
                    Amount = request.Amount,
                    OrderId = walletTransaction.Id,
                    OrderInfo = $"{walletTransaction.Id}",
                    ReturnUrl = _vnPayService.GetReturnUrl("WalletTransaction"),
                    IpAddress = "127.0.0.1"
                };

                string paymentUrl = _vnPayService.CreatePaymentUrl(vnPayRequest);

                // 4. Commit transaction
                await transaction.CommitAsync();

                return new Result<WalletTransaction>
                {
                    Success = true,
                    Message = paymentUrl,
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Result<WalletTransaction>
                {
                    Success = false,
                    Message = $"Failed to payment: {ex.Message}"
                };
            }
        }



        public async Task<Result<WalletTransaction>> PaymentReturn(WalletTransactionStatus request)
        {
            var walletTransaction = await _walletTransactionRepository.GetWalletTransactionById(request.WalletTransactionID);
            if (walletTransaction == null)
            {
                return new Result<WalletTransaction>
                {
                    Success = false,
                    Message = "WalletTransaction not found.",
                };
            }

            // Ghi lại mã giao dịch tại VNPAY
            walletTransaction.PaymentReferenceId = request.vnp_TransactionNo;

            if (request.Status == "00") // Thanh toán thành công
            {
                walletTransaction.Status = "SUCCESS";

                // Lấy UserWallet
                var userWallet = await _userWalletRepository.GetWalletById(walletTransaction.UserWalletId);
                if (userWallet == null)
                {
                    return new Result<WalletTransaction>
                    {
                        Success = false,
                        Message = "UserWallet not found.",
                    };
                }

                // Cộng tiền
                userWallet.Balance ??= 0; // Nếu null thì gán 0
                userWallet.Balance += walletTransaction.Amount ?? 0;

                // Cập nhật ví
                await _userWalletRepository.Update(userWallet);
            }
            else if (request.Status == "24")
            {
                walletTransaction.Status = "CANCELED";
            }
            else if (request.Status == "02")
            {
                walletTransaction.Status = "FAILED";
            }
            else
            {
                walletTransaction.Status = "UNKNOWN";
            }

            // Cập nhật giao dịch
            await _walletTransactionRepository.Update(walletTransaction);

            return new Result<WalletTransaction>
            {
                Success = true,
                Message = walletTransaction.Status,
                Data = walletTransaction
            };
        }

        public async Task<Result<List<WalletTransactionGetAllResponse>>> GetListTransactionForCurrentUser(int page, int pageSize)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new Result<List<WalletTransactionGetAllResponse>>
                    {
                        Success = false,
                        Message = "User not authenticated."
                    };
                }

                var userWallet = await _userWalletRepository.GetUserWalletById(userId);
                if (userWallet == null)
                {
                    return new Result<List<WalletTransactionGetAllResponse>>
                    {
                        Success = false,
                        Message = "User wallet not found."
                    };
                }

                var transactions = await _walletTransactionRepository
                    .GetWalletTransactionByUser(userWallet.Id, page, pageSize);

                return new Result<List<WalletTransactionGetAllResponse>>
                {
                    Success = true,
                    Data = _mapper.Map<List<WalletTransactionGetAllResponse>>(transactions)
                };
            }
            catch (Exception ex)
            {
                return new Result<List<WalletTransactionGetAllResponse>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

    }
}
