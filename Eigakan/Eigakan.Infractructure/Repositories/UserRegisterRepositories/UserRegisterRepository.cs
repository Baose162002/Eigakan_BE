
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;

namespace Eigakan.Infractructure.Repositories.UserRegisterRepositories
{
	public class UserRegisterRepository : GenericBase<UserRegister>, IUserRegisterRepository
	{
		public async Task<List<UserRegister>> GetAllUserRegisterAsync(int page, int pageSize, string? status, string? name)
		{
			return (await Get(
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				includeProperties: "User",
				filter: q => (string.IsNullOrEmpty(status) || q.Status == status) &&
                     (string.IsNullOrEmpty(name) || q.FullName.Contains(name)),
                pageIndex: page,
				pageSize: pageSize
			))		
			.ToList();
		}

		public async Task<int> CountAllUserRegisterAsync()
		{
			return await CountAsync();
		}

		public async Task<int> CountAllUserRegisterAcceptedAsync()
		{
			return await CountAsync(p => p.Status == "ACCEPTED");
		}

		public async Task<UserRegister> GetUserRegisterById(string id)
		{
			return await GetSingle(u => u.Id.Equals(id));
		}

		public async Task<List<UserRegister>> GetUserRegisterByEmail(string email)
		{
			return (await Get(u => u.Email.ToLower().Equals(email.ToLower()))).OrderByDescending(u => u.CreateDate).ToList();
		}
	}
}