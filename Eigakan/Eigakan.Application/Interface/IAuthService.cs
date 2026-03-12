using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AuthRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IAuthService
	{
		Task<Result<User>> Register(RegisterRequest user);
		Task<Result<object>> Login(LoginRequest request);
		Task<Result<User>> Verify(string token);
		Task<Result<User>> ForgotPassword(ForgotPasswordRequest forgotPassword);
		Task<Result<User>> ResetPassword(ResetPasswordRequest request);
	}
}
