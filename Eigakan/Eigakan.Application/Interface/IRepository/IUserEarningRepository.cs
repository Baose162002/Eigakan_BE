using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IUserEarningRepository : IGenericRepository<UserEarning>
	{
		Task<List<UserEarning>> GetAllUserEarningAsync(int page, int pageSize);
		Task<List<UserEarning>> GetAllUserEarningNoPaging();
		Task<List<UserEarning>> GetAllUserEarningAsyncNoPagingByUserId(string userId);
		Task<int> CountAllUserEarningAsync();
		Task<int> CountAllUserEarningByUserId(string userId);
		Task<UserEarning> GetUserEarningById(string id);
		Task<List<UserEarning>> GetAllUserEarningByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string userId);
	}
}
