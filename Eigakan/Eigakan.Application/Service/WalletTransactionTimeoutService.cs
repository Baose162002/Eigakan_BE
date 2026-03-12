using Eigakan.Application.Interface.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class WalletTransactionTimeoutService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public WalletTransactionTimeoutService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var transactionRepo = scope.ServiceProvider.GetRequiredService<IWalletTransactionRepository>();

                var utcNow = DateTime.UtcNow;  // Lấy giờ UTC hiện tại
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone); // Chuyển sang giờ Việt Nam

                // Lấy các giao dịch "PENDING" tạo trước 1 phút
                var timeoutThreshold = vietnamTime.AddMinutes(-1);
                var pendingTransactions = await transactionRepo.GetPendingTransactionsBefore(timeoutThreshold);

                foreach (var transaction in pendingTransactions)
                {
                    transaction.Status = "TIMEOUT";
                    await transactionRepo.Update(transaction);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
