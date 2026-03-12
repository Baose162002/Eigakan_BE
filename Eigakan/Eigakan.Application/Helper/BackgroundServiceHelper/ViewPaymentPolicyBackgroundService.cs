using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Helper.BackgroundServiecHelper
{
	public class ViewPaymentPolicyBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;

		private readonly ILogger<SubscriptionStatusBackgroundService> _logger;
		private readonly TimeSpan _interval = TimeSpan.FromDays(1);

		public ViewPaymentPolicyBackgroundService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			//tạo scope tạm để xử lí
			using (var scope = _scopeFactory.CreateScope())
			{
				try
				{
					var viewPaymentPolicyService = scope.ServiceProvider.GetRequiredService<IViewPaymentPolicyService>();

					await viewPaymentPolicyService.UpdateStatusViewPolicy();
					_logger.LogInformation("Initial view policy status check completed at: {Time}", DateTime.UtcNow);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error in initial ViewPaymentPolicyBackgroundService execution");
				}
			}

			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(_interval, stoppingToken);

				using (var scope = _scopeFactory.CreateScope())
				{
					try
					{
						var viewPaymentPolicyService = scope.ServiceProvider.GetRequiredService<IViewPaymentPolicyService>();

						await viewPaymentPolicyService.UpdateStatusViewPolicy();
						_logger.LogInformation("Initial view policy status check completed at: {Time}", DateTime.UtcNow);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error in initial ViewPaymentPolicyBackgroundService execution");
					}
				}
			}
		}

	}

}
