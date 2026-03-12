using AutoMapper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.SubscriptionPackageRequest;
using Eigakan.Domain.Request.SubscriptionPurchaseRequest;
using Eigakan.Domain.Response;
using Eigakan.Domain.Response.SubscriptionPackageResponse;
using Eigakan.Domain.Response.SubscriptionPurchaseResponse;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class SubscriptionPurchaseService : ISubscriptionPurchaseService
	{
		private readonly ISubscriptionPurchaseRepository _subscriptionPurchaseRepository;
		private readonly IUserRepository _userRepository;
		private readonly ILogger<SubscriptionPurchaseService> _logger;
		private readonly IMapper _mapper;

		public SubscriptionPurchaseService(ISubscriptionPurchaseRepository subscriptionPurchaseRepository, IUserRepository userRepository,
											IMapper mapper, ILogger<SubscriptionPurchaseService> logger)
		{
			_subscriptionPurchaseRepository = subscriptionPurchaseRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Result<bool>> SavePurchaseAsync(SubscriptionPurchase subscriptionPurchase)
		{
			try
			{
				if (subscriptionPurchase == null)
				{
					return new Result<bool> { Success = false, Message = "SubscriptionPurchase is null." };
				}

				await _subscriptionPurchaseRepository.Insert(subscriptionPurchase);

				return new Result<bool> { Success = true, Message = "Purchase saved successfully", Data = true };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error saving purchase: {@SubscriptionPurchase}", subscriptionPurchase);
				return new Result<bool> { Success = false, Message = $"Error saving purchase: {ex.Message}" };
			}
		}

		public async Task<Result<UserGetAllResponse>> UpdateStatusUserSubscriptionPurchase(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
					return new Result<UserGetAllResponse> { Success = false, Message = "Id must not be null" };

				var user = await _userRepository.GetUserById(id);

				if (user == null)
					return new Result<UserGetAllResponse> { Success = false, Message = "User not found" };

				user.RoleId = "33AAA70C";
				await _userRepository.Update(user);

				return new Result<UserGetAllResponse>
				{
					Success = true,
					Data = _mapper.Map<UserGetAllResponse>(user),
				};
			}
			catch (Exception ex)
			{
				return new Result<UserGetAllResponse> { Success = false, Message = ex.Message };
			}
		}

        public async Task UpdateExpiredSubscriptions()
        {
            var utcNow = DateTime.UtcNow;  // Lấy giờ UTC hiện tại
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);  // Chuyển sang giờ Việt Nam

            var expiredSubscriptions = await _subscriptionPurchaseRepository.GetExpiredSubscriptions();

            foreach (var subscription in expiredSubscriptions)
            {
                // Kiểm tra ngày hết hạn của subscription
                if (subscription.ExpiredDate < vietnamTime)  // So sánh với giờ Việt Nam
                {
                    subscription.Status = "Expired";
                    await _subscriptionPurchaseRepository.Update(subscription);

                    var user = await _userRepository.GetUserById(subscription.UserId);
                    if (user != null)
                    {
                        var latestSubscription = await _subscriptionPurchaseRepository.GetLatestUserSubscription(user.Id);

                        // Kiểm tra subscription mới nhất
                        if (latestSubscription == null || latestSubscription.ExpiredDate < vietnamTime)  // So sánh giờ Việt Nam
                        {
                            user.RoleId = "43AAA70C";  // Cập nhật lại role của user
                            await _userRepository.Update(user);
                        }
                    }
                }
            }
        }


        public async Task<SubscriptionPurchase> GetLatestUserSubscription(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("GetLatestUserSubscription: userId is null or empty");
				return null;
			}

			var latestSubscription = await _subscriptionPurchaseRepository.GetLatestUserSubscription(userId);

			if (latestSubscription == null)
			{
				_logger.LogInformation($"No active subscription found for user: {userId}");
				return null;
			}

			return latestSubscription;
		}

		
		public async Task<Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total, int ActiveSubscriptionCount, decimal totalEarnings)>> 
			GetAllSubscriptionPurchaseAsync(
					int page, int pageSize, string? id, DateTime? startDate, DateTime? endDate, DateTime? expiredDate,
					decimal? totalPrice, string? status, string? subscriptionId, string? userId)
			{
			try
			{
				var listSubscriptionPurchase = await _subscriptionPurchaseRepository.GetAllSubscriptionPurchase(page, pageSize, id, startDate, endDate, expiredDate, totalPrice, status, subscriptionId, userId);

				var listSubscriptionPurchaseNoPaging = await _subscriptionPurchaseRepository.GetAllSubscriptionPurchaseNoPaging();

				var total = await _subscriptionPurchaseRepository.CountAllSubscriptionPackageAsync();

				var response = _mapper.Map<List<SubscriptionPurchaseGetAllResponse>>(listSubscriptionPurchase);

				var activeSubscriptionCount = listSubscriptionPurchaseNoPaging.Count(x => x.Status.Equals("Active", StringComparison.OrdinalIgnoreCase));

				decimal totalEarnings = listSubscriptionPurchaseNoPaging.Sum(x => x.TotalPrice ?? 0);

				return new Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total, int ActiveSubscriptionCount, decimal totalEarnings)>
				{
					Success = true,
					Message = "SubscriptionPurchase retrieved successfully.",
					Data = (response, total, activeSubscriptionCount, totalEarnings)
				};
			}
			catch (Exception ex)
			{

				return new Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total, int ActiveSubscriptionCount, decimal totalEarnings)>
				{
					Success = false,
					Message = $"An error occurred while retrieving SubscriptionPurchase: {ex.Message}",
					Data = (null, 0,0,0)
				};
			}
		}

		public async Task<Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total)>> GetAllSubscriptionPurchaseUser(string userId, int page, int pageSize)
		{
			try
			{
				if (page <= 0 || pageSize <= 0)
				{
					return new Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total)>
					{
						Success = false,
						Message = "Invalid page or pageSize values.",
						Data = (null, 0)
					};
				}

				// Kiểm tra user có tồn tại không
				var user = await _userRepository.GetUserById(userId);
				if (user == null)
				{
					return new Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total)>
					{
						Success = false,
						Message = "User not found.",
						Data = (null, 0)
					};
				}

				var listSubscriptionPackage = await _subscriptionPurchaseRepository.GetSubscriptionPurchaseUserById(userId, page, pageSize);

				if (listSubscriptionPackage == null || !listSubscriptionPackage.Any())
				{
					return new Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total)>
					{
						Success = false,
						Message = "No subscription purchases found for this user.",
						Data = (new List<SubscriptionPurchaseGetAllResponse>(), 0)
					};
				}

				var total = await _subscriptionPurchaseRepository.CountAllSubscriptionPackageAsync();

				var response = _mapper.Map<List<SubscriptionPurchaseGetAllResponse>>(listSubscriptionPackage);

				return new Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total)>
				{
					Success = true,
					Message = "SubscriptionPurchase retrieved successfully.",
					Data = (response, total)
				};
			}
			catch (Exception ex)
			{
				return new Result<(List<SubscriptionPurchaseGetAllResponse> SubscriptionPurchases, int Total)>
				{
					Success = false,
					Message = $"An error occurred while retrieving SubscriptionPurchase: {ex.Message}",
					Data = (null, 0)
				};
			}
		}

	}
}
