using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.MovieEarning;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class MovieEarningService : IMovieEarningService
	{
		private readonly IMapper _mapper;
		private readonly Logger _logger;
		private readonly IMovieEarningRepository _MovieEarningRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMoviesRepository _moviesRepository;
		private readonly IViewPaymentPolicyRepository _viewPaymentPolicyRepository;

		public MovieEarningService(IMapper mapper, Logger logger, IMovieEarningRepository userEariningRepository,
								  IHttpContextAccessor httpContextAccessor, IMoviesRepository moviesRepository,
								  IViewPaymentPolicyRepository viewPaymentPolicyRepository)
		{
			_mapper = mapper;
			_logger = logger;
			_MovieEarningRepository = userEariningRepository;
			_httpContextAccessor = httpContextAccessor;
			_moviesRepository = moviesRepository;
			_viewPaymentPolicyRepository = viewPaymentPolicyRepository;
		}

		public async Task<MovieEarningDashboardResponse> GetAllMovieEarningAsync(int page, int pageSize)
		{
			var totalItems = await _MovieEarningRepository.CountAllMovieEarningAsync();

			var listMovieEarning = await _MovieEarningRepository.GetAllMovieEarningAsync(page, pageSize);

			var listMovieEarningNoPaging = await _MovieEarningRepository.GetAllMovieEarningAsyncNoPaging();

			int totalView = listMovieEarningNoPaging.Sum(x => x.TotalView ?? 0);
			
			decimal totalEarnings = listMovieEarningNoPaging.Sum(x => x.TotalEarnings ?? 0);

			decimal totalEarningsMovieContract = listMovieEarningNoPaging
				.Where(x => x.Movie != null && x.Movie.IsContract == true && x.TotalEarnings != null)
				.Sum(x => x.TotalEarnings ?? 0);


			return new MovieEarningDashboardResponse
			{
				Total = totalItems,
				TotalView = totalView,
				TotalEarnings = totalEarnings,
				TotalEarningsMovieContract = totalEarningsMovieContract,
				MovieEarning = _mapper.Map<List<MovieEarningResponse>>(listMovieEarning)
			};
		}

		public async Task<(List<MovieEarningResponse> movieEarningMovieId, int total, decimal totalEarning)> GetAllMovieEarningByMovieId(int page, int pageSize, DateOnly? startDate, DateOnly? endDate,string movieId)
		{
			var existingMovie = await _moviesRepository.GetMovieById(movieId);
			if (existingMovie == null)
			{
				
				return (new List<MovieEarningResponse>(), 0, 0);
			}

			var listMovieEarning = await _MovieEarningRepository.GetAllMovieEarningByMovieId(page, pageSize, startDate, endDate, movieId)
									?? new List<MovieEarning>();

			var total = await _MovieEarningRepository.CountAllMovieEarningByMovieId(movieId);

			var listMovieEarningNoPaging = await _MovieEarningRepository
				.GetAllMovieEarningAsyncNoPagingByMovieId(movieId) ?? new List<MovieEarning>();

			decimal totalEarnings = listMovieEarningNoPaging.Sum(x => x.TotalEarnings ?? 0);

			return (_mapper.Map<List<MovieEarningResponse>>(listMovieEarning), total, totalEarnings);
		}

		public async Task<Result<MovieEarning>> GetMovieEarningById(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
					return new Result<MovieEarning> { Success = false, Message = "Id is not be null" };

				var userId = await _MovieEarningRepository.GetMovieEarningById(id);

				if (userId == null)
					return new Result<MovieEarning> { Success = false, Message = "Id does not exist" };

				return new Result<MovieEarning>
				{
					Success = true,
					Data = _mapper.Map<MovieEarning>(userId),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieEarning));
				return new Result<MovieEarning> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<MovieEarning>> GetMovieEarningDayByLogin()
		{
			var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);

			var listMovie = await _moviesRepository.GetAllMovieNotContractByLogin(UserId.Value);
			if (listMovie == null || !listMovie.Any())
			{
				return new Result<MovieEarning>
				{
					Success = false,
					Message = "No movies found for the user."
				};
			}

			var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));

			var listMovieToday = listMovie
				.SelectMany(m => m.MovieCounts)
				.Where(mc => mc.ViewDate == today)
				.ToList();

			var totalView = listMovieToday.Sum(mc => mc.ViewCount) ?? 0; // Nếu null, thay bằng 0

			int page = 1;
			int pageSize = 1000;

			var paymentPolicies = await _viewPaymentPolicyRepository.GetAllViewPaymentPolicyAsync(page, pageSize);

			var activeOrWaitingPolicy = paymentPolicies?.FirstOrDefault(p => p.Status == "ACTIVE" || p.Status == "WAITING-FOR-INACTIVE");

			if (activeOrWaitingPolicy == null)
			{
				return new Result<MovieEarning>
				{
					Success = false,
					Message = "No active or waiting payment policy found."
				};
			}

			decimal CalculateTotalPayment(int? viewCount)
			{
				return (viewCount ?? 0) * activeOrWaitingPolicy.PricePerView;
			}

			return new Result<MovieEarning>
			{
				Success = true,
				Data = new MovieEarning
				{
					TotalView = totalView,
					TotalEarnings = CalculateTotalPayment(totalView)
				}
			};
		}
	}
}