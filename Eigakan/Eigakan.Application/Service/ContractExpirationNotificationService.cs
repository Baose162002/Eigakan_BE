using Eigakan.Application.Interface.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eigakan.Application.Helper.EmailSetting;

namespace Eigakan.Application.Service
{
    public class ContractExpirationNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ContractExpirationNotificationService> _logger;

        public ContractExpirationNotificationService(IServiceScopeFactory scopeFactory, ILogger<ContractExpirationNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var contractRepo = scope.ServiceProvider.GetRequiredService<IContractRepository>();
                var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                try
                {
                    var contracts = await contractRepo.GetAllContractsForCheckExpirationAsync();

                    // Lấy giờ UTC và chuyển thành giờ Việt Nam (UTC+7)
                    var utcNow = DateTime.UtcNow;  // Lấy giờ UTC hiện tại
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone).Date;  // Chuyển sang giờ Việt Nam

                    foreach (var contract in contracts)
                    {
                        // Kiểm tra hợp đồng hết hạn trong 7 ngày
                        if (contract.EndDate.HasValue && contract.EndDate.Value.Date == vietnamNow.AddDays(7))
                        {
                            var user = await userRepo.GetUserById(contract.UserId);

                            var subject = "Contract Expiration Notice - 7 Days Left";

                            var userBody = $@"
<p>Dear {user.FullName},</p>
<p>This is a friendly reminder that your collaboration contract for the movie <strong>{contract.Movie.Title}</strong> with our platform <strong>Eigakan</strong> is set to expire on <strong>{contract.EndDate:dd/MM/yyyy}</strong>.</p>
<p>Please take the time to review the contract and renew it if you wish to continue our cooperation.</p>
<p>We greatly appreciate your partnership and look forward to continued collaboration.</p>
<p>Best regards,<br/>Eigakan Team</p>";

                            var adminBody = $@"
<p>Dear Admin,</p>
<p>The collaboration contract between user <strong>{user.FullName}</strong> and our website <strong>Eigakan</strong> for the movie <strong>{contract.Movie.Title}</strong> is scheduled to expire on <strong>{contract.EndDate:dd/MM/yyyy}</strong>.</p>
<p>This is an automatic system notification to help monitor and manage contract timelines.</p>";


                            await emailService.SendEmailAsync(new MailResponse
                            {
                                ToEmail = user.Email,
                                Subject = subject,
                                Body = userBody
                            });

                            await emailService.SendEmailAsync(new MailResponse
                            {
                                ToEmail = "btran43850@gmail.com",  // Change to admin email here
                                Subject = subject,
                                Body = adminBody
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking contract expirations.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Check daily
            }
        }

    }


}
