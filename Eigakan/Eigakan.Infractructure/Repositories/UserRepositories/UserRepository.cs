using Amazon.S3.Model;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRequest;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.UserRepositories
{
	public class UserRepository : GenericBase<User>, IUserRepository
	{

		public async Task<List<User>> GetAllUserAsync(int page, int pageSize,string? status,string? name,string? roleName)
		{
			return (await Get(
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
                filter: q => (string.IsNullOrEmpty(status) || q.Status == status) &&
                     (string.IsNullOrEmpty(name) || q.FullName.Contains(name))&&
                     (string.IsNullOrEmpty(roleName) || q.Role.Name==roleName),

                includeProperties: "Role,UserRegister",
				pageIndex: page,
				pageSize: pageSize
			)).ToList();
		}

		public async Task<int> CountAllUsersAsync()
		{
			return await CountAsync();
		}

		public async Task<User> GetUserByEmail(string email)
		{
			return await GetSingle(u => u.Email.ToLower().Equals(email.ToLower()), includeProperties: "Role");
		}

		public async Task<User> GetUserById(string id)
		{
			return await GetSingle(u => u.Id.Equals(id), includeProperties: "Role,UserRegister,Contracts,Movies");
		}

		public async Task<User> GetUserByToken(string token)
		{
			return await GetSingle(a => a.PasswordResetToken == token || a.VerificationToken == token);
		}

		public async Task<int> CountAllUsersActiveAsync()
		{
			return await CountAsync(p => p.Status == "NORMAL");
		}

	}
}
