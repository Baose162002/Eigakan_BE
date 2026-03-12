using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class RefundRemainingViewsService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RefundRemainingViewsService> _logger;

        public RefundRemainingViewsService(IServiceScopeFactory serviceScopeFactory, ILogger<RefundRemainingViewsService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var adPurchaseItemRepo = scope.ServiceProvider.GetRequiredService<IAdPurchaseItemRepository>();
                var adPurchaseTransactionRepo = scope.ServiceProvider.GetRequiredService<IAdPurchaseTransactionRepository>();
                var adPackageRepo = scope.ServiceProvider.GetRequiredService<IAdPackageRepository>();
                var userWalletRepo = scope.ServiceProvider.GetRequiredService<IUserWalletRepository>();
                var walletTransactionRepo = scope.ServiceProvider.GetRequiredService<IWalletTransactionRepository>();

                var thresholdDate = DateTime.UtcNow.AddHours(7);
                var expiredItems = await adPurchaseItemRepo.GetItemsWithRemainingViewsOlderThan(thresholdDate);

                foreach (var item in expiredItems)
                {
                    try
                    {
                        var transaction = await adPurchaseTransactionRepo.GetAdPurchaseTransactionById(item.AdPurchaseTransactionId);
                        if (transaction == null) continue;

                        var refundAmount = item.RemainingViews * item.PricePerView;
                        if (refundAmount <= 0) continue;

                        var userWallet = await userWalletRepo.GetUserWalletById(transaction.UserId);
                        if (userWallet == null) continue;

                        // Lấy tên gói quảng cáo
                        var adPackage = await adPackageRepo.GetAdPackageById(item.AdPackageId);
                        var packageName = adPackage?.PackageName ?? $"PackageId {item.AdPackageId}";

                        userWallet.Balance ??= 0;
                        userWallet.Balance += refundAmount;

                        var refundTransaction = new WalletTransaction
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserWalletId = userWallet.Id,
                            Amount = refundAmount,
                            Type = $"Refund AdPackage \"{packageName}\"",
                            PaymentMethod = "SYSTEM",
                            PaymentReferenceId = item.Id,
                            Status = "SUCCESS",
                            CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                        };

                        await userWalletRepo.Update(userWallet);
                        await walletTransactionRepo.Insert(refundTransaction);
                        await walletTransactionRepo.SaveChangeTransaction();

                        item.RefundedPrice = refundAmount;
                        item.Status = "REFUNDED";
                        await adPurchaseItemRepo.Update(item);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refunding AdPurchaseItem {ItemId}", item.Id);
                    }
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }


}
