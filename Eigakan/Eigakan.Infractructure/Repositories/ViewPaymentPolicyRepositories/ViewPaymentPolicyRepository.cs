using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.ViewPaymentPolicyRepositories
{
	public class ViewPaymentPolicyRepository : GenericBase<ViewPaymentPolicy>, IViewPaymentPolicyRepository
	{
		public async Task<List<ViewPaymentPolicy>> GetAllViewPaymentPolicyAsync(int page, int pageSize)
		{
			return (await Get(
				orderBy: q => q.OrderByDescending(u => u.EffectiveDate),
				pageIndex: page,
				pageSize: pageSize
			))
			.ToList();
		}

		public async Task<int> CountAllViewPaymentPolicyAsync()
		{
			return await CountAsync();
		}

		public async Task<ViewPaymentPolicy> GetViewPaymentPolicyById(string id)
		{
			return await GetSingle(u => u.Id.Equals(id));
		}

		public async Task<List<ViewPaymentPolicy>> GetViewPaymentPolicyPendingAndWaiting()
		{
			return (await Get(
				u => u.Status == "WAITING-FOR-INACTIVE" || u.Status == "PENDING",
				orderBy: q => q.OrderByDescending(u => u.EffectiveDate)
				))
				.ToList();
		}

		public async Task<ViewPaymentPolicy> GetViewPaymentPolicyActive()
		{
			return await GetSingle(u => u.Status == "ACTIVE");
		}

	}
}