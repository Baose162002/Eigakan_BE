using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.ViewPaymentPolicy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IViewPaymentPolicyService
	{
		Task<(List<ViewPaymentPolicy> Policies, int Total)> GetAllViewPaymentPolicyAsync(int page, int pageSize);
		Task<Result<ViewPaymentPolicy>> GetViewPaymentPolicyById(string id);
		Task<Result<List<ViewPaymentPolicy>>> GetListPolicyPendingAndWaiting();
		Task<Result<ViewPaymentPolicy>> CreateViewPaymentPolicy(ViewPaymentPolicyCreateRequest ViewPaymentPolicyCreateRequest);
		Task<Result<ViewPaymentPolicy>> UpdatePolicy(string id, ViewPaymentPolicyUpdateRequest viewPaymentPolicyUpdateRequest);
		Task<Result<ViewPaymentPolicy>> GetViewPaymentPolicyActive();
		Task UpdateStatusViewPolicy();
		Task<Result<ViewPaymentPolicy>> CancelPolicy();
	}
}
