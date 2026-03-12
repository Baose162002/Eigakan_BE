using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Response.AdminDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IAdminDashboardService
	{
		Task<Result<AdminDasboardOverallResponse>> DashboardAdminOverall();
	}
}
