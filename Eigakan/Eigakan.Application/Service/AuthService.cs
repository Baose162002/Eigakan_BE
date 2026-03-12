using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Interface;
using Microsoft.Extensions.Configuration;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AuthRequest;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Eigakan.Domain.Enum;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;

namespace Eigakan.Application.Service
{
	public class AuthService : IAuthService
	{
		private readonly IAuthRepository _authRepository;
		private readonly IConfiguration _configuration;
		private readonly IUserRepository _userRepository;
		private readonly IEmailService _emailService;
		private readonly Webhook _webhook;
		private readonly Logger _logger;
		private readonly PasswordSettings _passwordSettings;

		public AuthService(IAuthRepository authRepository, IConfiguration configuration, 
						   IUserRepository userRepository, IEmailService emailService,
						   Webhook webhook, PasswordSettings passwordSettings,
						   Logger logger )
		{
			_authRepository = authRepository;
			_configuration = configuration;
			_userRepository = userRepository;
			_emailService = emailService;
			_webhook = webhook;
			_passwordSettings = passwordSettings;
			_logger = logger;
		}

		public async Task<Result<User>> Register(RegisterRequest user)
		{
			var exstingUser = await _userRepository.GetUserByEmail(user.Email);

			if (exstingUser != null)
			{
				return new Result<User>
				{
					Success = false,
					Message = "User already exists"
				};
			}

			_passwordSettings.CreatePasswordHash(user.Password,
				out byte[] passwordHash,
				out byte[] passwordSalt);

			var newUser = new User
			{
				Id = Guid.NewGuid().ToString(),
				Email = user.Email,
				FullName = user.FullName,
				Birthday = "20-01-1989",
				RoleId = "43AAA70C",
				Picture = "https://res.cloudinary.com/dtihkfbuk/image/upload/v1727016986/d3da46fz2dpav0tniukw.jpg",
				CreateDate = DateTime.Now,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				Status = UserStatusEnum.INACTIVE.ToString(),
				VerificationToken = CreateRandomToken()
			};
			await _userRepository.Insert(newUser);

			string frontendreseturl = _configuration["FrontendSettings:VerifyAccountUrl"];

			string verifydUrl = $"{frontendreseturl}{newUser.VerificationToken}";

			// Tạo email với liên kết thực sự
			var mailrequest = new EmailSetting.MailResponse
			{
				ToEmail = newUser.Email,
				Subject = "Welcome to Eigakan",
				Body = $@"
    <div style='font-family:Arial, sans-serif; display:flex; justify-content:center;'>
  <div style='max-width: 600px; border: 1px solid #e0e0e0; border-radius: 10px; padding: 20px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
      <div style='text-align:center; padding:20px;'>
          <img src='https://res.cloudinary.com/dn8bn2sty/image/upload/v1739771796/image_vxdaik.png' alt='FFilms logo' style='width: 250px; margin-bottom:10px;'/>
      </div>
      <h2 style='text-align:center;'>Verify account</h2>
      <p style='text-align:center; color: red;'>{newUser.Email}</p>
      <p style='text-align:center;'>Please click the button to access our website </p>
      <div style='text-align:center;'>
          <a href='{verifydUrl}' style='background-color: rgb(241, 93, 93); color: black; padding: 10px 25px; text-decoration: none; border-radius: 5px; font-weight:bold;'>Verify Account</a>
      </div>
      <p style='text-align:center;'>Eigakan</p>
  </div>
</div>",

			};
			var emailTask = Task.Run(() => _emailService.SendEmailAsync(mailrequest));
			// Đợi các tác vụ không đồng bộ kết thúc
			await emailTask;

			return new Result<User>
			{
				Success = true,
				Message = "Create successfull"
			};
		}

		public async Task<Result<object>> Login(LoginRequest request)
		{
			var exstingUser = await _userRepository.GetUserByEmail(request.Email);

			if (exstingUser == null)
			{
				return new Result<object>
				{
					Success = false,
					Message = "User not exists"
				};
			}
			if (!_passwordSettings.VerifyPasswordHash(request.Password, exstingUser.PasswordHash, exstingUser.PasswordSalt))
			{
				return new Result<object>
				{
					Success = false,
					Message = "Password incorrect!!!"
				};
			}

			string token = CreateToken(exstingUser);

			var user = new
			{
				UserId = exstingUser.Id,
				FullName = exstingUser.FullName,
				Picture = exstingUser.Picture,
				RoleName = exstingUser.Role.Name,
			};

			return new Result<object>
			{
				Success = true,
				Data = user,
				Message = token
			};
		}

		public async Task<Result<User>> Verify(string token)
		{
			var exstingUser = await _userRepository.GetUserByToken(token);

			if (exstingUser == null)
			{
				return new Result<User>
				{
					Success = false,
					Message = "Token invalid"
				};
			}

			exstingUser.VerifiedAt = DateTime.Now;
			exstingUser.Status = UserStatusEnum.NORMAL.ToString();

			try
			{
				await _userRepository.Update(exstingUser);
				return new Result<User>
				{
					Success = true,
					Message = "Verify successfull"
				};
			}
			catch (Exception ex)
			{

				return new Result<User>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<Result<User>> ForgotPassword(ForgotPasswordRequest forgotPassword)
		{
			var exstingEmail = await _userRepository.GetUserByEmail(forgotPassword.Email);

			if (exstingEmail == null)
			{
				await _webhook.Send("Email not exist");
				return new Result<User>
				{
					Success = false,
					Message = "Email not exist"
				};
			}

			// update db và gửi email
			string token = CreateRandomToken();

			exstingEmail.PasswordResetToken = token;
			exstingEmail.ResetTokenExpirex = DateTime.Now.AddDays(1);

			await _authRepository.Update(exstingEmail);

			string frontendreseturl = _configuration["FrontendSettings:ResetPasswordUrl"];
			string resetPasswordUrl = $"{frontendreseturl}{token}";

			// Tạo email với liên kết thực sự
			var mailrequest = new EmailSetting.MailResponse
			{
				ToEmail = forgotPassword.Email,
				Subject = "Reset Password",
				Body = $@"
    <div style='font-family:Arial, sans-serif; display:flex; justify-content:center;'>
  <div style='max-width: 600px; border: 1px solid #e0e0e0; border-radius: 10px; padding: 20px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
      <div style='text-align:center; padding:20px;'>
          <img src='https://res.cloudinary.com/dn8bn2sty/image/upload/v1739771796/image_vxdaik.png' alt='FFilms logo' style='width: 250px; margin-bottom:10px;'/>
      </div>
      <h2 style='text-align:center;'>Reset password</h2>
      <p style='text-align:center; color: red;'>{forgotPassword.Email}</p>
      <p style='text-align:center;'>Please click the button below to reset your password </p>
      <div style='text-align:center;'>
          <a href='{resetPasswordUrl}' style='background-color: rgb(241, 93, 93); color: black; padding: 10px 25px; text-decoration: none; border-radius: 5px; font-weight:bold;'>Reset password</a>
      </div>
      <p style='text-align:center;'>Eigakan</p>
  </div>
</div>",

			};

			await _emailService.SendEmailAsync(mailrequest);

			return new Result<User>
			{
				Success = true,
				Message = "Send mail successfull"
			};					
		}

		public async Task<Result<User>> ResetPassword(ResetPasswordRequest request)
		{
			var checkToken = await _userRepository.GetUserByToken(request.Token);

			if (checkToken == null || checkToken.ResetTokenExpirex < DateTime.Now)
			{
				return new Result<User>
				{
					Success = false,
					Message = "Token invalid"
				};
			}

			_passwordSettings.CreatePasswordHash(request.Password,
				out byte[] passwordHash,
				out byte[] passwordSalt);

			checkToken.PasswordHash = passwordHash;
			checkToken.PasswordSalt = passwordSalt;
			checkToken.ResetTokenExpirex = null;
			checkToken.PasswordResetToken = null;

			try
			{
				await _authRepository.Update(checkToken);
				return new Result<User>
				{
					Success = true,
					Message = "Reset password successfull"
				};
			}
			catch(Exception ex)
			{
				return new Result<User>
				{
					Success = false,
					Message = "Reset password fail"
				};
			}
			
		}


		private string CreateToken(User user)
		{
			string roleName = string.Empty;
			if (user.RoleId == "13AAA70C")
			{
				roleName = "PUBLISHER";
			}
			else if (user.RoleId == "23AAA70C")
			{
				roleName = "ADVERTISER";
			}
			else if (user.RoleId == "33AAA70C")
			{
				roleName = "VIP MEMBER";
			}else if (user.RoleId == "43AAA70C")
			{
				roleName = "MEMBER";
			}else if (user.RoleId == "53AAA70C")
			{
				roleName = "ADMIN";
			}else if (user.RoleId == "63AAA70C")
			{
				roleName = "MANAGER";
			}

			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, roleName),
				new Claim(ClaimTypes.Name, user.FullName ?? "Unknown"),
				new Claim(MySetting.CLAIM_USERID, user.Id)
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
				_configuration.GetSection("AppSettings:Token").Value));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddMonths(1),
				signingCredentials: creds);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}

		private string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
		}
	}

}