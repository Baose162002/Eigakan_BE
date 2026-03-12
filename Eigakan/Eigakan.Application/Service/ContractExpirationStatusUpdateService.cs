using Eigakan.Application.Interface.IRepository;
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
    public class ContractExpirationStatusUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ContractExpirationStatusUpdateService> _logger;

        public ContractExpirationStatusUpdateService(IServiceProvider serviceProvider, ILogger<ContractExpirationStatusUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var contractRepo = scope.ServiceProvider.GetRequiredService<IContractRepository>();
                    var movieRepo = scope.ServiceProvider.GetRequiredService<IMoviesRepository>();

                    // Lấy giờ UTC và chuyển sang giờ Việt Nam (UTC+7)
                    var utcNow = DateTime.UtcNow;
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone); // giờ Việt Nam

                    // Lấy tất cả các hợp đồng đã hết hạn
                    var allExpiredContracts = await contractRepo.GetExpiredContractsAsync();

                    foreach (var contract in allExpiredContracts)
                    {
                        try
                        {
                            // Kiểm tra nếu hợp đồng đã hết hạn và cập nhật trạng thái hợp đồng
                            if (contract.EndDate.HasValue && contract.EndDate.Value.Date <= vietnamNow.Date)
                            {
                                await contractRepo.UpdateContractStatusAsync(contract.Id, "EXPIRED");

                                var movie = await movieRepo.GetMovieById(contract.MovieId);
                                if (movie != null && movie.Status != "INACTIVE")
                                {
                                    // Cập nhật trạng thái bộ phim nếu chưa là INACTIVE
                                    await movieRepo.UpdateMovieStatusAsync(movie.Id, "INACTIVE");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error updating status for contract {contract.Id}");
                        }
                    }

                }

                // Chạy mỗi 24 tiếng
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

    }

}
