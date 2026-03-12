using Eigakan.Application.Interface;
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
    public class RoomCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RoomCleanupService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("[RoomCleanupService] Checking for expired rooms...");

                using (var scope = _serviceScopeFactory.CreateScope()) // Tạo một scope mới
                {
                    var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                    await roomService.EndExpiredRoomsAsync(); // Gọi service trong scope mới
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Kiểm tra mỗi phút
            }
        }
    }
}
