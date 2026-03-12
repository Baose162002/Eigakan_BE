using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AuthRequest;
using Xunit;

namespace UnitTest.Service
{
	public class AuthServiceTest
	{
		private readonly Mock<IAuthRepository> _authRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IEmailService> _emailServiceMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<IOptions<DiscordWebhookUrls>> _optionsMock;
		private readonly Mock<Webhook> _webhookMock;
		private readonly Logger _logger;
		private readonly PasswordSettings _passwordSettings;
		private readonly AuthService _authService;

		public AuthServiceTest()
		{
			_authRepositoryMock = new Mock<IAuthRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_emailServiceMock = new Mock<IEmailService>();
			_configurationMock = new Mock<IConfiguration>();
			_optionsMock = new Mock<IOptions<DiscordWebhookUrls>>();

			_optionsMock.Setup(o => o.Value).Returns(new DiscordWebhookUrls
			{
				AdminUpdates = "https://discord.com/api/webhooks/123456789"
			});

			_webhookMock = new Mock<Webhook>(_optionsMock.Object);
			_logger = new Logger(_webhookMock.Object);
			_passwordSettings = new PasswordSettings();

			_configurationMock.Setup(c => c["FrontendSettings:VerifyAccountUrl"]).Returns("https://verifylink.com/");
			_configurationMock.Setup(c => c["FrontendSettings:ResetPasswordUrl"]).Returns("https://resetlink.com/");
			_configurationMock.Setup(c => c.GetSection("AppSettings:Token").Value).Returns("dummy_jwt_token_key_1234567890123456");

			_authService = new AuthService(
				_authRepositoryMock.Object,
				_configurationMock.Object,
				_userRepositoryMock.Object,
				_emailServiceMock.Object,
				_webhookMock.Object,
				_passwordSettings,
				_logger
			);
		}

		#region Register

		[Fact]
		public async Task Register_Should_Return_Fail_When_EmailExists()
		{
			var request = new RegisterRequest { Email = "test@example.com", Password = "123456", FullName = "Test User" };
			_userRepositoryMock.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync(new User());
			var result = await _authService.Register(request);
			Assert.False(result.Success);
			Assert.Equal("User already exists", result.Message);
		}

		[Fact]
		public async Task Register_Should_Return_Success_When_User_Does_Not_Exist()
		{
			var request = new RegisterRequest { Email = "new@example.com", Password = "123456", FullName = "New User" };
			_userRepositoryMock.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync((User)null);
			_userRepositoryMock.Setup(r => r.Insert(It.IsAny<User>())).Returns(Task.CompletedTask);
			_emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<EmailSetting.MailResponse>())).Returns(Task.CompletedTask);
			var result = await _authService.Register(request);
			Assert.True(result.Success);
			Assert.Equal("Create successfull", result.Message);
		}

		#endregion

		#region Login

		[Fact]
		public async Task Login_Should_Return_Fail_When_User_Does_Not_Exist()
		{
			var request = new LoginRequest { Email = "nouser@example.com", Password = "abc" };
			_userRepositoryMock.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync((User)null);
			var result = await _authService.Login(request);
			Assert.False(result.Success);
			Assert.Equal("User not exists", result.Message);
		}

		[Fact]
		public async Task Login_Should_Return_Fail_When_Password_Is_Wrong()
		{
			_passwordSettings.CreatePasswordHash("correct", out var hash, out var salt);
			var request = new LoginRequest { Email = "user@example.com", Password = "wrong" };
			var user = new User { Email = request.Email, PasswordHash = hash, PasswordSalt = salt };
			_userRepositoryMock.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync(user);
			var result = await _authService.Login(request);
			Assert.False(result.Success);
			Assert.Equal("Password incorrect!!!", result.Message);
		}

		//[Fact]
		//public async Task Login_Should_Return_Success_When_Credentials_Are_Correct()
		//{
		//	_passwordSettings.CreatePasswordHash("123456", out var hash, out var salt);
		//	var request = new LoginRequest { Email = "user@example.com", Password = "123456" };
		//	var user = new User
		//	{
		//		Id = "u1",
		//		Email = request.Email,
		//		FullName = "User",
		//		Role = new Role { Name = "MEMBER" },
		//		Picture = "img.jpg",
		//		PasswordHash = hash,
		//		PasswordSalt = salt,
		//		RoleId = "43AAA70C"
		//	};
		//	_userRepositoryMock.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync(user);
		//	var result = await _authService.Login(request);
		//	Assert.True(result.Success);
		//	Assert.Equal("u1", ((dynamic)result.Data).UserId);
		//}

		#endregion

		#region ForgotPassword

	

		[Fact]
		public async Task ForgotPassword_Should_Send_Email_And_Save_Token()
		{
			var request = new ForgotPasswordRequest { Email = "user@email.com" };
			var user = new User { Email = request.Email };
			_userRepositoryMock.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync(user);
			_authRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
			_emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<EmailSetting.MailResponse>())).Returns(Task.CompletedTask);
			var result = await _authService.ForgotPassword(request);
			Assert.True(result.Success);
			Assert.Equal("Send mail successfull", result.Message);
		}

		#endregion

		#region ResetPassword

		[Fact]
		public async Task ResetPassword_Should_Fail_When_Token_Invalid()
		{
			var request = new ResetPasswordRequest { Token = "invalid", Password = "pass" };
			_userRepositoryMock.Setup(r => r.GetUserByToken(request.Token)).ReturnsAsync((User)null);
			var result = await _authService.ResetPassword(request);
			Assert.False(result.Success);
			Assert.Equal("Token invalid", result.Message);
		}

		[Fact]
		public async Task ResetPassword_Should_Fail_When_Token_Expired()
		{
			var request = new ResetPasswordRequest { Token = "expired", Password = "pass" };
			var user = new User { ResetTokenExpirex = DateTime.UtcNow.AddMinutes(-1), PasswordResetToken = "expired" };
			_userRepositoryMock.Setup(r => r.GetUserByToken(request.Token)).ReturnsAsync(user);
			var result = await _authService.ResetPassword(request);
			Assert.False(result.Success);
			Assert.Equal("Token invalid", result.Message);
		}

		//[Fact]
		//public async Task ResetPassword_Should_Update_Password_And_Clear_Token()
		//{
		//	var request = new ResetPasswordRequest { Token = "valid", Password = "pass123" };
		//	var user = new User { ResetTokenExpirex = DateTime.UtcNow.AddMinutes(10), PasswordResetToken = "valid" };
		//	_userRepositoryMock.Setup(r => r.GetUserByToken(request.Token)).ReturnsAsync(user);
		//	_authRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
		//	var result = await _authService.ResetPassword(request);
		//	Assert.True(result.Success);
		//	Assert.Equal("Reset password successfull", result.Message);
		//	Assert.Null(user.PasswordResetToken);
		//	Assert.Null(user.ResetTokenExpirex);
		//}

		#endregion
	}
}
