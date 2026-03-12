using Aspose.Pdf.Operators;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class EarningCalculationBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;

		public EarningCalculationBackgroundService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				Console.WriteLine("[EarningCalculationBackgroundService] Service is running");

				while (!stoppingToken.IsCancellationRequested)
				{
					using var scope = _scopeFactory.CreateScope();
					var logger = scope.ServiceProvider.GetRequiredService<Logger>();

					var movieEarningRepo = scope.ServiceProvider.GetRequiredService<IMovieEarningRepository>();
					var userEarningRepo = scope.ServiceProvider.GetRequiredService<IUserEarningRepository>();
					var movieRepo = scope.ServiceProvider.GetRequiredService<IMoviesRepository>();
					var policyRepo = scope.ServiceProvider.GetRequiredService<IViewPaymentPolicyRepository>();

					var today = GetLocalDate();
					//var today = new DateOnly(2025, 4, 22); 


					if (IsProcessingDay(today.Day))
					{
						var (startDate, endDate) = GetWeekRange(today);

						await using var transaction = await movieEarningRepo.BeginTransactionAsync();

						var exsitingMovieEarning = await movieEarningRepo.GetListMovieEarningByDate(startDate, endDate);
						if (exsitingMovieEarning != null && exsitingMovieEarning.Count > 0)
						{
							Console.WriteLine("[EarningCalculationBackgroundService] already calculate for this week");
							await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
							continue;
						}

						try
						{
							var movieEarnings = await CalculateMovieEarnings(movieRepo, policyRepo, startDate, endDate);
							await movieEarningRepo.BulkInsertAsync(movieEarnings);

							var userEarnings = await CalculateUserEarnings(movieEarningRepo, policyRepo, startDate, endDate);
							await userEarningRepo.BulkInsertAsync(userEarnings);


							await logger.LogAnnoucement(userEarnings, $"Statistic earning movie and user, {startDate} - {endDate}");
							await transaction.CommitAsync();
						}
						catch (Exception ex)
						{
							await logger.LogError(ex, "Error while calculate earning");
							await transaction.RollbackAsync();
						}
					}

					await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[EarningCalculationBackgroundService] Fatal error: {ex.Message}");
			}

		}

		private async Task<List<MovieEarning>> CalculateMovieEarnings(
			IMoviesRepository movieRepo,
			IViewPaymentPolicyRepository policyRepo,
			DateOnly start,
			DateOnly end)
		{
			var movieStats = await movieRepo.GetListMovieByDate(start, end);
			var policy = await GetActivePolicy(policyRepo);

			var result = new List<MovieEarning>();

			foreach (var movie in movieStats)
			{
				var totalView = movie.MovieCounts
					.Where(mc => mc.ViewDate >= start && mc.ViewDate <= end)
					.Sum(mc => mc.ViewCount);

				var totalEarning = totalView * policy.PricePerView;

				result.Add(new MovieEarning
				{
					Id = Guid.NewGuid().ToString(),
					MovieId = movie.Id,
					UserId = movie.UserId,
					StartWeek = start,
					EndWeek = end,
					TotalView = totalView,
					TotalEarnings = totalEarning,
					CreateDate = GetLocalDateTime(),
					Status = true
				});
			}

			return result;
		}

		private async Task<List<UserEarning>> CalculateUserEarnings(
			IMovieEarningRepository movieEarningRepo,
			IViewPaymentPolicyRepository policyRepo,
			DateOnly start,
			DateOnly end)
		{
			var movieEarnings = await movieEarningRepo.GetListMovieEarningByDate(start, end);
			var policy = await GetActivePolicy(policyRepo);

			var result = new List<UserEarning>();

			foreach (var group in movieEarnings
				 .Where(m => m.Movie != null && m.Movie.IsContract == false)
				 .GroupBy(m => m.UserId))
			{
				var totalViews = group.Sum(m => m.TotalView);               
				var totalEarning = totalViews * policy.PricePerView;         
				var webShare = totalEarning * policy.WebSharePercentage / 100m; 
				var final = totalEarning - webShare;


				result.Add(new UserEarning
				{
					Id = Guid.NewGuid().ToString(),
					UserId = group.Key,
					StartWeek = start,
					EndWeek = end,
					TotalView = totalViews,
					TotalEarnings = totalEarning,
					WebEarnings = webShare,
					FinalEarnings = final,
					CreateDate = GetLocalDateTime()
				});
			}

			return result;
		}

		private static bool IsProcessingDay(int day) => day == 1 || day == 8 || day == 15 || day == 22;

		private static DateOnly GetLocalDate() =>
			DateOnly.FromDateTime(GetLocalDateTime());

		private static DateTime GetLocalDateTime() =>
			TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

		private static async Task<ViewPaymentPolicy> GetActivePolicy(IViewPaymentPolicyRepository repo)
		{
			var policies = await repo.GetAllViewPaymentPolicyAsync(1, 1000);
			var policy = policies.FirstOrDefault(p => p.Status == "ACTIVE" || p.Status == "WAITING-FOR-INACTIVE");

			if (policy == null)
				throw new InvalidOperationException("Not policy found");

			return policy;
		}

		private static (DateOnly Start, DateOnly End) GetWeekRange(DateOnly today)
		{
			return today.Day switch
			{
				1 => (new DateOnly(today.Year, today.Month, 1).AddDays(-10), new DateOnly(today.Year, today.Month, 1).AddDays(-1)),
				8 => (new DateOnly(today.Year, today.Month, 1), new DateOnly(today.Year, today.Month, 7)),
				15 => (new DateOnly(today.Year, today.Month, 8), new DateOnly(today.Year, today.Month, 14)),
				22 => (new DateOnly(today.Year, today.Month, 15), new DateOnly(today.Year, today.Month, 21)),
				_ => throw new InvalidOperationException("Chỉ xử lý vào ngày 1, 8, 15, 22")
			};
		}
	}
}
