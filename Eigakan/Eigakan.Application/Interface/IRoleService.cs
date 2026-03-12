using CloudinaryDotNet.Actions;
using Eigakan.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IRoleService
	{
		Task<(List<Domain.Models.Role> Roles, int Total)> GetAllRoleAsync(int page, int pageSize);
	}
}
