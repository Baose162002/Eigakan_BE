using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IUserRegisterRepository : IGenericRepository<UserRegister>
	{
		Task<List<UserRegister>> GetAllUserRegisterAsync(int page, int pageSize, string? status, string? name);
		Task<int> CountAllUserRegisterAsync();
		Task<int> CountAllUserRegisterAcceptedAsync();
		Task<UserRegister> GetUserRegisterById(string id);
		Task<List<UserRegister>> GetUserRegisterByEmail(string email);
	}
}
