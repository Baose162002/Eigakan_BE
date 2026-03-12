using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRequest;
using Eigakan.Domain.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Eigakan.Application.Helper.EmailSetting;

namespace UnitTest.Service
{
	public class UserServiceTest
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IUserRegisterRepository> _userRegisterMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
		private readonly Mock<IEmailService> _emailServiceMock;
		private readonly Mock<IOptions<DiscordWebhookUrls>> _optionsMock;  
		private readonly Mock<Webhook> _webhookMock; 
		private readonly UserService _userService;
		private readonly UserRegisterService _userRegisterService;
		private Mock<IAmazonS3> _s3ClientMock;

		public UserServiceTest()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_mapperMock = new Mock<IMapper>();
			_httpContextAccessorMock = new Mock<IHttpContextAccessor>();
			_emailServiceMock = new Mock<IEmailService>();
			_userRegisterMock = new Mock<IUserRegisterRepository>();
			_s3ClientMock = new Mock<IAmazonS3>();

			// Mock user context
			var context = new DefaultHttpContext();
			context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim("UserId", "user123"),
				new Claim(ClaimTypes.Role, "ADMIN")
			}));
			_httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

			// Mock IConfiguration with AWS settings
			var configurationMock = new Mock<IConfiguration>();
			configurationMock.Setup(c => c["AWS:AccessKey"]).Returns("mockAccessKey");
			configurationMock.Setup(c => c["AWS:SecretKey"]).Returns("mockSecretKey");
			configurationMock.Setup(c => c["AWS:Region"]).Returns("us-west-2");

			// Mock Webhook (use the mock IOptions)
			var optionsMock = new Mock<IOptions<DiscordWebhookUrls>>();
			optionsMock.Setup(o => o.Value).Returns(new DiscordWebhookUrls { AdminUpdates = "https://discord.com/api/webhooks/1326070733650137139/qG7-RocMHVj0CFlw3vAr2YGE2Ou7QJfOA9jwzL7TgZj0JJTgYkSUBFg_yfWMWPYhjLs-" });

			// Mock Webhook
			var webhookMock = new Mock<Webhook>(optionsMock.Object);

			// Mock Logger with Webhook dependency
			var logger = new Logger(webhookMock.Object);

			// Create UserService with mocked dependencies
			_userService = new UserService(
				_userRepositoryMock.Object,
				_mapperMock.Object,
				_httpContextAccessorMock.Object,
				new PasswordSettings(),
				configurationMock.Object, // Use the mocked configuration here
				_emailServiceMock.Object,
				webhookMock.Object,
				logger,
				_userRegisterMock.Object
			);
		}


		#region UpdateUser
		[Fact]
		public async Task UpdateUser_Should_Return_Success_When_User_Exists_And_Valid()
		{
			// Arrange
			var userId = "user123";
			var existingUser = new User { Id = userId, FullName = "Old Name" };
			var updateRequest = new UserUpdateRequest { FullName = "New Name" };

			_userRepositoryMock.Setup(r => r.GetUserById(userId))
				.ReturnsAsync(existingUser);

			// Act
			var result = await _userService.UpdateUser(userId, updateRequest);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Update user successfull", result.Message);
			Assert.Equal("New Name", existingUser.FullName);
		}

		[Fact]
		public async Task UpdateUser_Should_Return_Success_When_User_Id_Matches()
		{
			// Arrange
			var userId = "user12";
			var roleClaim = "USER";
			var existingUser = new User { Id = userId, FullName = "Old Name" };
			var userUpdateRequest = new UserUpdateRequest { FullName = "New Name" };

			_httpContextAccessorMock.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, userId));
			_httpContextAccessorMock.Setup(x => x.HttpContext.User.FindFirst(ClaimTypes.Role)).Returns(new Claim(ClaimTypes.Role, roleClaim));
			_userRepositoryMock.Setup(x => x.GetUserById(userId)).ReturnsAsync(existingUser);

			// Act
			var result = await _userService.UpdateUser(userId, userUpdateRequest);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Update user successfull", result.Message);
			Assert.Equal("New Name", existingUser.FullName);
		}

		[Fact]
		public async Task UpdateUser_Should_Return_Success_When_User_Is_Admin()
		{
			// Arrange
			var userId = "user123";
			var roleClaim = "ADMIN";
			var existingUser = new User { Id = "user456", FullName = "Old Name" };
			var userUpdateRequest = new UserUpdateRequest { FullName = "New Name" };

			_httpContextAccessorMock.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, userId));
			_httpContextAccessorMock.Setup(x => x.HttpContext.User.FindFirst(ClaimTypes.Role)).Returns(new Claim(ClaimTypes.Role, roleClaim));
			_userRepositoryMock.Setup(x => x.GetUserById(existingUser.Id)).ReturnsAsync(existingUser);

			// Act
			var result = await _userService.UpdateUser(existingUser.Id, userUpdateRequest);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Update user successfull", result.Message);
			Assert.Equal("New Name", existingUser.FullName);
		}

		[Fact]
		public async Task UpdateUser_Should_Return_Failure_When_User_Does_Not_Have_Permission()
		{
			// Arrange
			var userId = "user123";
			var roleClaim = "USER";
			var existingUser = new User { Id = "user456", FullName = "Old Name" };
			var userUpdateRequest = new UserUpdateRequest { FullName = "New Name" };

			_httpContextAccessorMock.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, userId));
			_httpContextAccessorMock.Setup(x => x.HttpContext.User.FindFirst(ClaimTypes.Role)).Returns(new Claim(ClaimTypes.Role, roleClaim));
			_userRepositoryMock.Setup(x => x.GetUserById(existingUser.Id)).ReturnsAsync(existingUser);

			// Act
			var result = await _userService.UpdateUser(existingUser.Id, userUpdateRequest);

			// Assert
			Assert.False(result.Success);
			Assert.Equal("You do not have permission to update this user", result.Message);
		}

		#endregion

		#region CreateUser

		[Fact]
		public async Task CreateUser_Should_Return_Failure_When_Email_Already_Registered()
		{
			
			var userCreateRequest = new UserCreateRequest
			{
				Email = "existinguser@example.com",
				FullName = "Existing User",
				RoleId = "role1"
			};

			
			var existingUser = new User
			{
				Email = userCreateRequest.Email,
				FullName = userCreateRequest.FullName,
				RoleId = userCreateRequest.RoleId
			};

			_userRepositoryMock.Setup(r => r.GetUserByEmail(userCreateRequest.Email)).ReturnsAsync(existingUser);

			
			var result = await _userService.CreateUser(userCreateRequest);

			
			Assert.False(result.Success);
			Assert.Equal("Email has been registered and cannot join this program", result.Message);
		}

		[Fact]
		public async Task CreateUser_Should_Return_Success_When_Creating_New_User()
		{
			
			var userCreateRequest = new UserCreateRequest
			{
				Email = "newuser@example.com",
				FullName = "New User",
				RoleId = "role1"
			};

		
			_userRepositoryMock.Setup(r => r.GetUserByEmail(userCreateRequest.Email)).ReturnsAsync((User)null);

			
			_userRepositoryMock.Setup(r => r.Insert(It.IsAny<User>())).Returns(Task.CompletedTask);

		
			var newUser = new User
			{
				Id = "newUserId",
				Email = userCreateRequest.Email,
				FullName = userCreateRequest.FullName
			};
			_userRepositoryMock.Setup(r => r.GetUserById(It.IsAny<string>())).ReturnsAsync(newUser);

			
			var userGetAllResponse = new UserGetAllResponse
			{
				Email = userCreateRequest.Email,
				FullName = userCreateRequest.FullName
			};
			_mapperMock.Setup(m => m.Map<UserGetAllResponse>(It.IsAny<User>())).Returns(userGetAllResponse);

			
			var result = await _userService.CreateUser(userCreateRequest);

			
			Assert.True(result.Success);
			Assert.Equal("Create successful", result.Message);
			Assert.Equal(userCreateRequest.Email, result.Data.Email);
			Assert.Equal(userCreateRequest.FullName, result.Data.FullName);
		}




		#endregion

		#region handle status user

		[Fact]
		public async Task ChangeStatusUser_Should_Set_Normal_When_Status_0()
		{
			// Arrange
			var user = new User
			{
				Id = "user123",
				Status = UserStatusEnum.INACTIVE.ToString()
			};

			_userRepositoryMock.Setup(x => x.GetUserById("user123"))
				.ReturnsAsync(user);

			_userRepositoryMock.Setup(x => x.Update(It.IsAny<User>()))
				.Returns(Task.CompletedTask);

			var request = new UserStatusRequest
			{
				Id = "user123",
				Status = 0
			};

			// Act
			var result = await _userService.ChangeStatusUser(request);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Update status successfull", result.Message);
			Assert.Equal(UserStatusEnum.NORMAL.ToString(), user.Status);
			_userRepositoryMock.Verify(x => x.Update(It.Is<User>(u => u.Status == UserStatusEnum.NORMAL.ToString())), Times.Once);
		}

		[Fact]
		public async Task ChangeStatusUser_Should_ReturnError_When_UserNotFound()
		{
			_userRepositoryMock.Setup(r => r.GetUserById("notfound")).ReturnsAsync((User)null);

			var request = new UserStatusRequest { Id = "notfound", Status = 0 };

			var result = await _userService.ChangeStatusUser(request);

			Assert.False(result.Success);
			Assert.Equal("Id does not exist", result.Message);
			_userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
		}


		#endregion

	}
}
