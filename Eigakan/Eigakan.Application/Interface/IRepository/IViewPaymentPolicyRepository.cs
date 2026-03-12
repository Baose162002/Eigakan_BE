using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IViewPaymentPolicyRepository : IGenericRepository<ViewPaymentPolicy>
	{
		Task<List<ViewPaymentPolicy>> GetAllViewPaymentPolicyAsync(int page, int pageSize);
		Task<int> CountAllViewPaymentPolicyAsync();
		Task<ViewPaymentPolicy> GetViewPaymentPolicyById(string id);
		Task<ViewPaymentPolicy> GetViewPaymentPolicyActive();
		Task<List<ViewPaymentPolicy>> GetViewPaymentPolicyPendingAndWaiting();
	}
}
