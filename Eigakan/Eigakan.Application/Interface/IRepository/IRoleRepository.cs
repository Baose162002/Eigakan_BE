using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IRoleRepository : IGenericRepository<Role>
	{
		Task<List<Role>> GetAllRoleAsync(int page, int pageSize);
		Task<int> CountAllRolesAsync();
	}
}
