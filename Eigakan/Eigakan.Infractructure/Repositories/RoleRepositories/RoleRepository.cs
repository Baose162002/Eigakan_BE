using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.RoleRepositories
{
	public class RoleRepository : GenericBase<Role>, IRoleRepository
	{

		public async Task<List<Role>> GetAllRoleAsync(int page, int pageSize)
		{
			return (await Get(
				pageIndex: page,
				pageSize: pageSize
			)).ToList();
		}

		public async Task<int> CountAllRolesAsync()
		{
			return await CountAsync();
		}
	}
}
