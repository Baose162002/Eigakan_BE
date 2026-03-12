using Eigakan.Application.Interface;
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
    public class SubscriptionStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
      
        private readonly ILogger<SubscriptionStatusBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); 

        public SubscriptionStatusBackgroundService(IServiceScopeFactory scopeFactory, ILogger<SubscriptionStatusBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //tạo scope tạm để xử lí
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                   
                    var subscriptionPurchaseService = scope.ServiceProvider.GetRequiredService<ISubscriptionPurchaseService>();

					
					await subscriptionPurchaseService.UpdateExpiredSubscriptions(); 
					_logger.LogInformation("Initial subscription status check completed at: {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in initial SubscriptionStatusBackgroundService execution");
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);

                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        var subscriptionPurchaseService = scope.ServiceProvider.GetRequiredService<ISubscriptionPurchaseService>();
						

						await subscriptionPurchaseService.UpdateExpiredSubscriptions();
                        _logger.LogInformation("Subscription status check completed at: {Time}", DateTime.UtcNow);

					}
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in SubscriptionStatusBackgroundService");
                    }
                }
            }
        }

    }

}
