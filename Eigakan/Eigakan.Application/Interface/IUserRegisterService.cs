using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRegisterRequest;
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
	public interface IUserRegisterService 
	{
		Task<(List<UserRegister> Users, int Total)> GetAllUserRegisterAsync(int page, int pageSize, string? status, string? name);
		Task<Result<UserRegister>> GetUserRegisterById(string id);
		Task<Result<UserRegister>> AcceptedUserRegister(AcceptedUserRegisterRequest acceptedUserRegisterRequest);
		Task<Result<UserRegister>> RejectedUserRegister(RejectedUserRegisterRequest rejectedUserRegisterRequest);
		Task<Result<UserRegister>> CreateUserRegister(UserRegisterCreateRequest userRegisterCreateRequest);
		Task<List<UserRegister>> GetAllUserRegisterAsyncByEmail(string email);
	}
}
