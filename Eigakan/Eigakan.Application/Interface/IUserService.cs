using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRequest;
using Eigakan.Domain.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IUserService
	{
		Task<(List<UserGetAllResponse> Users, int Total)> GetAllUserAsync(int page, int pageSize, string? status, string? name,string? roleName);
		Task<Result<UserGetAllResponse>> GetUserById(string id);
		Task<Result<UserGetAllResponse>> GetUserByEmail(string email);
		Task<Result<UserGetAllResponse>> ChangeStatusUser(UserStatusRequest userStatusRequest);
		Task<Result<UserGetAllResponse>> UpdateUser(string id, UserUpdateRequest userUpdateRequest);
		Task<Result<UserGetAllResponse>> GetUserByLogin();
		Task<Result<UserGetAllResponse>> CreateUserByRegister(UserCreateRequest userCreateRequest);
		Task<Result<UserGetAllResponse>> CreateUser(UserCreateRequest userCreateRequest);
	}
}
