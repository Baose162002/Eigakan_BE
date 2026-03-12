using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.MovieHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class MovieCountService : IMovieCountService
	{
		private readonly IMovieCountRepository _movieCountRepository;
		private readonly Logger _logger;
		private readonly IViewPaymentPolicyRepository _viewPaymentPolicyRepository;

		public MovieCountService(IMovieCountRepository movieCountRepository, Logger logger,
								 IViewPaymentPolicyRepository viewPaymentPolicyRepository)
		{
			_movieCountRepository = movieCountRepository;
			_logger = logger;
			_viewPaymentPolicyRepository = viewPaymentPolicyRepository;
		}

		public async Task<Result<MovieCount>> GetMovieCountByMovieId(string? movieId)
		{
			try
			{
				var movie = await _movieCountRepository.GetMovieCountByMovieId(movieId);
				if (movie != null)
				{
					return new Result<MovieCount>
					{
						Success = true,
						Data = movie
					};
				}
				return new Result<MovieCount>
				{
					Success = false,
					Message = "Not Found MovieCount!!"
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieCount));
				return new Result<MovieCount> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<MovieCount>> IncreaseMovieCount(MovieHistoryCreateRequest movieCount)
		{
			try
			{
				var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));

				var movieView = await _movieCountRepository.CheckCountByMovieDate(movieCount.MovieId, today);
				if (movieView != null)
				{
					movieView.ViewCount += 1;

					await _movieCountRepository.Update(movieView);
				}
				else
				{
					movieView = new MovieCount
					{
						Id = Guid.NewGuid().ToString(),
						MovieId = movieCount.MovieId,
						ViewCount = 1,
						ViewDate = today
					};
					await _movieCountRepository.Insert(movieView);
				}
				return new Result<MovieCount>
				{
					Success = true,
					Message = "View Count Updated!!!",
					Data = movieView
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieCount));
				return new Result<MovieCount> { Success = false, Message = ex.Message };
			}
		}

		public async Task<object> GetMovieViewStatistics(string movieId)
		{
			var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));

			var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Thứ 2 đầu tuần
			var startOfMonth = new DateOnly(today.Year, today.Month, 1);


			var movieCounts = await _movieCountRepository.GetAllMovieCountByMovieId(movieId);

			var result = new
			{
				Today = movieCounts.Where(m => m.ViewDate == today).Sum(m => m.ViewCount),
				ThisWeek = movieCounts.Where(m => m.ViewDate >= startOfWeek && m.ViewDate <= today).Sum(m => m.ViewCount),
				ThisMonth = movieCounts.Where(m => m.ViewDate >= startOfMonth && m.ViewDate <= today).Sum(m => m.ViewCount),
				Total = movieCounts.Sum(m => m.ViewCount)
			};

			return result;
		}

		//public async Task<object> GetMovieViewStatistics(string movieId)
		//{
		//	var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));


		//	var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Thứ 2 đầu tuần
		//	var startOfMonth = new DateOnly(today.Year, today.Month, 1); // Ngày đầu tháng


		//	var movieCounts = await _movieCountRepository.GetAllMovieCountByMovieId(movieId);

		//	int page = 1;
		//	int pageSize = 1000;

		//	var paymentPolicies = await _viewPaymentPolicyRepository.GetAllViewPaymentPolicyAsync(page, pageSize);

		//	var activeOrWaitingPolicy = paymentPolicies.FirstOrDefault(p => p.Status == "ACTIVE" || p.Status == "WAITING-FOR-INACTIVE");

		//	if (activeOrWaitingPolicy == null)
		//	{
		//		throw new InvalidOperationException("No active or waiting payment policy found.");
		//	}


		//	var todayViewCount = movieCounts.Where(m => m.ViewDate == today).Sum(m => m.ViewCount ?? 0); 
		//	var thisWeekViewCount = movieCounts.Where(m => m.ViewDate >= startOfWeek && m.ViewDate <= today).Sum(m => m.ViewCount ?? 0);
		//	var thisMonthViewCount = movieCounts.Where(m => m.ViewDate >= startOfMonth && m.ViewDate <= today).Sum(m => m.ViewCount ?? 0);


		//	decimal CalculateTotalPayment(int? viewCount)
		//	{
		//		return (viewCount ?? 0) * activeOrWaitingPolicy.PricePerView; // Nếu viewCount là null, thay bằng 0
		//	}

		//	var result = new
		//	{
		//		Total = new { ViewCount = movieCounts.Sum(m => m.ViewCount ?? 0), Amount = CalculateTotalPayment(movieCounts.Sum(m => m.ViewCount ?? 0)) },
		//		Today = new { ViewCount = todayViewCount, Amount = CalculateTotalPayment(todayViewCount) },
		//		ThisWeek = new { ViewCount = thisWeekViewCount, Amount = CalculateTotalPayment(thisWeekViewCount) },
		//		ThisMonth = new { ViewCount = thisMonthViewCount, Amount = CalculateTotalPayment(thisMonthViewCount) }
		//	};

		//	return result;
		//}



	}
}
