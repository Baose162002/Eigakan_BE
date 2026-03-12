using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.UserEariningRepositories
{
	public class UserEarningRepository : GenericBase<UserEarning>, IUserEarningRepository
	{
		public async Task<List<UserEarning>> GetAllUserEarningAsync(int page, int pageSize)
		{
			return (await Get(
				includeProperties: "User",
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				pageIndex: page,
				pageSize: pageSize
			))
			.ToList();
		}

		public async Task<List<UserEarning>> GetAllUserEarningNoPaging()
		{
			return (await Get(
				includeProperties: "User"
			))
			.ToList();
		}

		public async Task<List<UserEarning>> GetAllUserEarningAsyncNoPagingByUserId(string userId)
		{
			return (await Get(
				includeProperties: "User",
				filter: q => q.UserId == userId
			))
			.ToList();
		}

		public async Task<int> CountAllUserEarningAsync()
		{
			return await CountAsync();
		}
		
		public async Task<int> CountAllUserEarningByUserId(string userId)
		{
			return await CountAsync(q => q.UserId == userId);
		}

		public async Task<UserEarning> GetUserEarningById(string id)
		{
			return await GetSingle(u => u.Id.Equals(id), includeProperties: "User");
		}

		public async Task<List<UserEarning>> GetAllUserEarningByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string userId)
		{
			return (await Get(
				includeProperties: "User",
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				filter: q =>
					q.UserId == userId &&
					(!startDate.HasValue || q.StartWeek == startDate.Value) &&
					(!endDate.HasValue || q.EndWeek == endDate.Value),
				pageIndex: page,
				pageSize: pageSize
			)).ToList();
		}


	}
}