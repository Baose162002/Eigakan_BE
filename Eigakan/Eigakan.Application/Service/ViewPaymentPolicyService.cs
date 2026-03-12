using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.ViewPaymentPolicy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eigakan.Application.Helper.Logging;
using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Domain.Request.UserRequest;
using Eigakan.Domain.Response;
using System.Security.Claims;

namespace Eigakan.Application.Service
{
	public class ViewPaymentPolicyService : IViewPaymentPolicyService
	{
		private readonly IViewPaymentPolicyRepository _viewPaymentPolicyRepository;
		private readonly Logger _logger;
		private readonly IMapper _mapper;

		public ViewPaymentPolicyService(IViewPaymentPolicyRepository viewPaymentPolicyRepository, Logger logger,
										IMapper mapper)
		{
			_viewPaymentPolicyRepository = viewPaymentPolicyRepository;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<(List<ViewPaymentPolicy> Policies, int Total)> GetAllViewPaymentPolicyAsync(int page, int pageSize)
		{
			// Lấy danh sách user với phân trang
			var listPolicy = await _viewPaymentPolicyRepository.GetAllViewPaymentPolicyAsync(page, pageSize);

			// Đếm tổng số lượng user
			var total = await _viewPaymentPolicyRepository.CountAllViewPaymentPolicyAsync();

			// Trả về dữ liệu và tổng số lượng
			return (_mapper.Map<List<ViewPaymentPolicy>>(listPolicy), total);
		}

		public async Task<Result<ViewPaymentPolicy>> GetViewPaymentPolicyById(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
					return new Result<ViewPaymentPolicy> { Success = false, Message = "Id is not be null" };

				var userId = await _viewPaymentPolicyRepository.GetViewPaymentPolicyById(id);

				if (userId == null)
					return new Result<ViewPaymentPolicy> { Success = false, Message = "Id does not exist" };

				return new Result<ViewPaymentPolicy>
				{
					Success = true,
					Data = _mapper.Map<ViewPaymentPolicy>(userId),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(ViewPaymentPolicy));
				return new Result<ViewPaymentPolicy> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<ViewPaymentPolicy>> GetViewPaymentPolicyActive()
		{
			try
			{
				var policy = await _viewPaymentPolicyRepository.GetViewPaymentPolicyActive();

				if (policy == null)
					return new Result<ViewPaymentPolicy> { Success = true, Message = "Not Found" , Data = policy };

				return new Result<ViewPaymentPolicy>
				{
					Success = true,
					Data = _mapper.Map<ViewPaymentPolicy>(policy),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(ViewPaymentPolicy));
				return new Result<ViewPaymentPolicy> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<List<ViewPaymentPolicy>>> GetListPolicyPendingAndWaiting()
		{
			var activePolicy = await _viewPaymentPolicyRepository.GetViewPaymentPolicyPendingAndWaiting();

			if (!activePolicy.Any()) 
			{
				return new Result<List<ViewPaymentPolicy>> { Success = true, Message = "No active policy", Data = new List<ViewPaymentPolicy>() };
			}

			return new Result<List<ViewPaymentPolicy>> { Success = true, Data = activePolicy };
		}

		public async Task<Result<ViewPaymentPolicy>> CreateViewPaymentPolicy(ViewPaymentPolicyCreateRequest ViewPaymentPolicyCreateRequest)
		{
			try
			{
				// Lấy ngày của EffectiveDate
				var effectiveDay = ViewPaymentPolicyCreateRequest.EffectiveDate.Value.Day;

				
				var allowedDays = new HashSet<int> { 1, 8, 15, 22 };
				if (!allowedDays.Contains(effectiveDay))
				{
					return new Result<ViewPaymentPolicy>
					{
						Success = false,
						Message = "Effective date must be on the 1st, 8th, 15th or 22nd of the month."
					};
				}

				var listPolicy = await _viewPaymentPolicyRepository.GetViewPaymentPolicyPendingAndWaiting();
				if (listPolicy.Count > 1)
				{
					return new Result<ViewPaymentPolicy> { Success = false, Message = "There is already a policy waiting for inactive." };
				}

				var activePolicy = await _viewPaymentPolicyRepository.GetViewPaymentPolicyActive();
				if (activePolicy != null)
				{
					// Chuyển DateOnly thành DateTime và tính sự khác biệt ngày
					var dateDifference = (ViewPaymentPolicyCreateRequest.EffectiveDate.Value.ToDateTime(new TimeOnly()) - activePolicy.EffectiveDate.Value.ToDateTime(new TimeOnly())).Days;

					if (dateDifference < 7)
					{
						return new Result<ViewPaymentPolicy> { Success = false, Message = "The new policy's effective date must be at least 1 week after the previous policy." };
					}

					activePolicy.Status = "WAITING-FOR-INACTIVE";
					await _viewPaymentPolicyRepository.Update(activePolicy);
				}

				var newPolicy = new ViewPaymentPolicy()
				{
					Id = Guid.NewGuid().ToString(),
					EffectiveDate = ViewPaymentPolicyCreateRequest.EffectiveDate,
					PricePerView = ViewPaymentPolicyCreateRequest.PricePerView,
					WebSharePercentage = ViewPaymentPolicyCreateRequest.WebSharePercentage,
					Status = "PENDING"
				};
				await _viewPaymentPolicyRepository.Insert(newPolicy);
				return new Result<ViewPaymentPolicy> { Success = true, Message = "Create payment policy successful", Data = newPolicy };

			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(ViewPaymentPolicy));
				return new Result<ViewPaymentPolicy> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<ViewPaymentPolicy>> UpdatePolicy(string id, ViewPaymentPolicyUpdateRequest viewPaymentPolicyUpdateRequest)
		{
			try
			{
				var existingPolicy = await _viewPaymentPolicyRepository.GetViewPaymentPolicyById(id);

				if (existingPolicy.Id != null && existingPolicy.Status == "PENDING")
				{
					existingPolicy.PricePerView = viewPaymentPolicyUpdateRequest.PricePerView;
					existingPolicy.WebSharePercentage = viewPaymentPolicyUpdateRequest.WebSharePercentage;

					await _viewPaymentPolicyRepository.Update(existingPolicy);
					await _logger.LogAnnoucement(existingPolicy, nameof(ViewPaymentPolicy));
					return new Result<ViewPaymentPolicy> { Data = _mapper.Map<ViewPaymentPolicy>(existingPolicy), Success = true, Message = "Update policy successfull" };

				}
				return new Result<ViewPaymentPolicy> { Success = false, Message = "Can not update policy right now!!" };

			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(ViewPaymentPolicy));
				return new Result<ViewPaymentPolicy> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<ViewPaymentPolicy>> CancelPolicy()
		{
			try
			{
				var existingPolicy = await _viewPaymentPolicyRepository.GetViewPaymentPolicyPendingAndWaiting();
				
				if (existingPolicy.Count == 0)
				{
					return new Result<ViewPaymentPolicy> { Success = false, Message = "Can not update policy right now!!" };
				}

				var waitingPolicy = existingPolicy.FirstOrDefault(p => p.Status == "WAITING-FOR-INACTIVE");
				var pendingPolicy = existingPolicy.FirstOrDefault(p => p.Status == "PENDING");

				waitingPolicy.Status = "ACTIVE";
				await _viewPaymentPolicyRepository.Update(waitingPolicy);


				pendingPolicy.Status = "INACTIVE";
				await _viewPaymentPolicyRepository.Update(pendingPolicy);
			
				return new Result<ViewPaymentPolicy> { Success = true, Message = "Cancel policy successfull" };
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(ViewPaymentPolicy));
				return new Result<ViewPaymentPolicy> { Success = false, Message = ex.Message };
			}
		}

		public async Task UpdateStatusViewPolicy()
		{
			var viewPolicies = await _viewPaymentPolicyRepository.GetViewPaymentPolicyPendingAndWaiting();

			if (viewPolicies == null || viewPolicies.Count != 2)
				return;

			var waitingPolicy = viewPolicies.FirstOrDefault(p => p.Status == "WAITING-FOR-INACTIVE");
			var pendingPolicy = viewPolicies.FirstOrDefault(p => p.Status == "PENDING");

			if (pendingPolicy == null || waitingPolicy == null)
				return;


			if (pendingPolicy.EffectiveDate == DateOnly.FromDateTime(DateTime.UtcNow))
			{
				
				waitingPolicy.Status = "INACTIVE";
				await _viewPaymentPolicyRepository.Update(waitingPolicy);

				
				pendingPolicy.Status = "ACTIVE";
				await _viewPaymentPolicyRepository.Update(pendingPolicy);
			}
		}

	}
}
