using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRequest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IUserRepository : IGenericRepository<User>
	{
		Task<List<User>> GetAllUserAsync(int page, int pageSize, string? status, string? name,string? roleName);
		Task<User> GetUserById(string id);
		Task<User> GetUserByEmail(string email);
		Task<User> GetUserByToken(string token);
		Task<int> CountAllUsersAsync();
		Task<int> CountAllUsersActiveAsync();

	}
}
