using AutoMapper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Interface;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Helper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Spreadsheet;
using Eigakan.Domain.Response.UserEarning;
using Eigakan.Domain.Response.UserEarning;
using Eigakan.Domain.Response.MovieEarning;

namespace Eigakan.Application.Service
{
	public class UserEarningService : IUserEarningService
	{
		private readonly IMapper _mapper;
		private readonly Logger _logger;
		private readonly IUserEarningRepository _userEarningRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IUserRepository _userRepository;
		private readonly IViewPaymentPolicyRepository _viewPaymentPolicyRepository;

		public UserEarningService(IMapper mapper, Logger logger, IUserEarningRepository userEariningRepository,
								  IHttpContextAccessor httpContextAccessor, IUserRepository userRepository,
								  IViewPaymentPolicyRepository viewPaymentPolicyRepository)
		{
			_mapper = mapper;
			_logger = logger;
			_userEarningRepository = userEariningRepository;
			_httpContextAccessor = httpContextAccessor;
			_userRepository = userRepository;
			_viewPaymentPolicyRepository = viewPaymentPolicyRepository;
		}

		public async Task<UserEarningDashboardResponse> GetAllUserEarningAsync(int page, int pageSize)
		{
			var totalItems = await _userEarningRepository.CountAllUserEarningAsync();

			var listUserEarning = await _userEarningRepository.GetAllUserEarningAsync(page, pageSize);

			var listUserEarningNoPaging = await _userEarningRepository.GetAllUserEarningNoPaging();

			decimal totalEarnings = listUserEarningNoPaging.Sum(x => x.TotalEarnings ?? 0);

			decimal webEarnings = listUserEarningNoPaging.Sum(x => x.WebEarnings ?? 0);

			decimal finalEarning = listUserEarning.Sum(x => x.FinalEarnings ?? 0);


			return new UserEarningDashboardResponse
			{
				Total = totalItems,
				TotalEarnings = totalEarnings,
				WebEarnings = webEarnings,
				FinalEarnings = finalEarning,
				userEarnings = _mapper.Map<List<UserEarningResponse>>(listUserEarning)
			};
		}

		public async Task<Result<UserEarningResponse>> GetUserEarningById(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
					return new Result<UserEarningResponse> { Success = false, Message = "Id is not be null" };

				var userId = await _userEarningRepository.GetUserEarningById(id);

				if (userId == null)
					return new Result<UserEarningResponse> { Success = false, Message = "Id does not exist" };

				return new Result<UserEarningResponse>
				{
					Success = true,
					Data = _mapper.Map<UserEarningResponse>(userId),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(UserEarning));
				return new Result<UserEarningResponse> { Success = false, Message = ex.Message };
			}
		}

		//public async Task<Result<UserEarningResponse>> GetUserEarningDayByLogin()
		//{
		//	var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);

		//	var listUser = await _userRepository.GetAllUserNotContractByLogin(UserId.Value);
		//	if (listUser == null || !listUser.Any())
		//	{
		//		return new Result<UserEarningResponse>
		//		{
		//			Success = false,
		//			Message = "No Users found for the user."
		//		};
		//	}

		//	var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));

		//	var listUserToday = listUser
		//		.SelectMany(m => m.UserCounts)
		//		.Where(mc => mc.ViewDate == today)
		//		.ToList();

		//	var totalView = listUserToday.Sum(mc => mc.ViewCount) ?? 0; // Nếu null, thay bằng 0

		//	int page = 1;
		//	int pageSize = 1000;

		//	var paymentPolicies = await _viewPaymentPolicyRepository.GetAllViewPaymentPolicyAsync(page, pageSize);

		//	var activeOrWaitingPolicy = paymentPolicies?.FirstOrDefault(p => p.Status == "ACTIVE" || p.Status == "WAITING-FOR-INACTIVE");

		//	if (activeOrWaitingPolicy == null)
		//	{
		//		return new Result<UserEarningResponse>
		//		{
		//			Success = false,
		//			Message = "No active or waiting payment policy found."
		//		};
		//	}

		//	decimal CalculateTotalPayment(int? viewCount)
		//	{
		//		return (viewCount ?? 0) * activeOrWaitingPolicy.PricePerView;
		//	}

		//	return new Result<UserEarningResponse>
		//	{
		//		Success = true,
		//		Data = new UserEarningResponse
		//		{
		//			TotalView = totalView,
		//			TotalEarnings = CalculateTotalPayment(totalView)
		//		}
		//	};
		//}

		public async Task<(List<UserEarningResponse> UserEarningLogin, int Total)> GetAllUserEarningAsyncByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate)
		{

			var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
			if (string.IsNullOrEmpty(UserId?.Value))
				return (new List<UserEarningResponse>(), 0);

			var listUserEarning = await _userEarningRepository.GetAllUserEarningByLogin(page, pageSize, startDate, endDate, UserId.Value);


			var total = await _userEarningRepository.CountAllUserEarningByUserId(UserId.Value);


			return (_mapper.Map<List<UserEarningResponse>>(listUserEarning), total);
		}

		public async Task<(List<UserEarningResponse> userEarningUserId, int total, decimal totalEarning , decimal finalEarning)> GetAllUserEarningByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate)
		{
			var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
			if (string.IsNullOrEmpty(UserId?.Value))
				return (new List<UserEarningResponse>(), 0, 0,0);

			var listUserEarning = await _userEarningRepository.GetAllUserEarningByLogin(page, pageSize, startDate, endDate, UserId.Value)
									?? new List<UserEarning>();

			var total = await _userEarningRepository.CountAllUserEarningByUserId(UserId.Value);

			var listUserEarningNoPaging = await _userEarningRepository
				.GetAllUserEarningAsyncNoPagingByUserId(UserId.Value) ?? new List<UserEarning>();

			decimal totalEarnings = listUserEarningNoPaging.Sum(x => x.TotalEarnings ?? 0);
			decimal finalEarnings = listUserEarning.Sum(x => x.FinalEarnings ?? 0);

			return (_mapper.Map<List<UserEarningResponse>>(listUserEarning), total, totalEarnings,finalEarnings);
		}

		public async Task<(List<UserEarningResponse> userEarningUserId, int total, decimal totalEarning, decimal finalEarning)> GetAllUserEarningAsyncByUserId(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string userId)
		{
			
			var listUserEarning = await _userEarningRepository.GetAllUserEarningByLogin(page, pageSize, startDate, endDate, userId)
									?? new List<UserEarning>();

			var total = await _userEarningRepository.CountAllUserEarningByUserId(userId);

			var listUserEarningNoPaging = await _userEarningRepository
				.GetAllUserEarningAsyncNoPagingByUserId(userId) ?? new List<UserEarning>();

			decimal totalEarnings = listUserEarningNoPaging.Sum(x => x.TotalEarnings ?? 0);
			decimal finalEarnings = listUserEarning.Sum(x => x.FinalEarnings ?? 0);

			return (_mapper.Map<List<UserEarningResponse>>(listUserEarning), total, totalEarnings, finalEarnings);
		}
	
	}
}
