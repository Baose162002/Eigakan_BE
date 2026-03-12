using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRegisterRequest;
using Eigakan.Domain.Response.UserEarning;


namespace Eigakan.Application.Interface
{
	public interface IUserEarningService
	{
		Task<UserEarningDashboardResponse> GetAllUserEarningAsync(int page, int pageSize);
		Task<Result<UserEarningResponse>> GetUserEarningById(string id);
		//Task<Result<UserEarningResponse>> GetUserEarningDayByLogin();
		Task<(List<UserEarningResponse> userEarningUserId, int total, decimal totalEarning, decimal finalEarning)> GetAllUserEarningByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate);
		Task<(List<UserEarningResponse> userEarningUserId, int total, decimal totalEarning, decimal finalEarning)> GetAllUserEarningAsyncByUserId(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string userId);
	}
}
