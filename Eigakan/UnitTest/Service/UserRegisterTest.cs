using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRegisterRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
	public class UserRegisterTest
	{
		private readonly Mock<IUserRegisterRepository> _userRegisterMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IOptions<DiscordWebhookUrls>> _optionsMock;
		private readonly Mock<Webhook> _webhookMock;
		private readonly UserRegisterService _userRegisterService;
		private readonly Mock<IUserRepository> _userRepositoryMock;

		public UserRegisterTest()
		{
			_mapperMock = new Mock<IMapper>();
			_userRegisterMock = new Mock<IUserRegisterRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_optionsMock = new Mock<IOptions<DiscordWebhookUrls>>();
			_optionsMock.Setup(o => o.Value).Returns(new DiscordWebhookUrls
			{
				AdminUpdates = "https://discord.com/api/webhooks/1326070733650137139/qG7-RocMHVj0CFlw3vAr2YGE2Ou7QJfOA9jwzL7TgZj0JJTgYkSUBFg_yfWMWPYhjLs-"
			});

			_webhookMock = new Mock<Webhook>(_optionsMock.Object);

			var logger = new Logger(_webhookMock.Object);

			_userRegisterService = new UserRegisterService(
				_userRegisterMock.Object,
				_mapperMock.Object,
				_userRepositoryMock.Object,
				logger
		);
		}

		#region handle status
		[Fact]
		public async Task AcceptedUserRegister_Should_Return_Success_When_Valid()
		{

			var userRegisterId = "register123";
			var userRegister = new UserRegister
			{
				Id = userRegisterId,
				Status = UserRegisterEnum.REVIEWING.ToString()
			};

			_userRegisterMock.Setup(x => x.GetUserRegisterById(userRegisterId))
				.ReturnsAsync(userRegister);

			_userRegisterMock.Setup(x => x.Update(It.IsAny<UserRegister>()))
				.Returns(Task.CompletedTask);

			var request = new AcceptedUserRegisterRequest { Id = userRegisterId };


			var result = await _userRegisterService.AcceptedUserRegister(request);


			Assert.True(result.Success);
			Assert.Equal("Update status successfull", result.Message);
			Assert.Equal(UserRegisterEnum.ACCEPTED.ToString(), userRegister.Status);

		}

		[Fact]
		public async Task RejectedUserRegister_Should_Return_Success_When_Valid()
		{

			var userRegisterId = "register123";
			var userRegister = new UserRegister
			{
				Id = userRegisterId,
				Status = UserRegisterEnum.REVIEWING.ToString()
			};

			_userRegisterMock.Setup(x => x.GetUserRegisterById(userRegisterId))
				.ReturnsAsync(userRegister);

			_userRegisterMock.Setup(x => x.Update(It.IsAny<UserRegister>()))
				.Returns(Task.CompletedTask);

			var request = new RejectedUserRegisterRequest { Id = userRegisterId, ReasonForRejection = "not suitable" };


			var result = await _userRegisterService.RejectedUserRegister(request);


			Assert.True(result.Success);
			Assert.Equal("Update status successfull", result.Message);
			Assert.Equal(UserRegisterEnum.REJECTED.ToString(), userRegister.Status);
		}

		#endregion

		#region createUserRegister

		[Fact]
		public async Task CreateUserRegister_Should_Return_Success_When_Valid()
		{
			// Arrange
			var createRequest = new UserRegisterCreateRequest
			{
				Email = "test@example.com",
				FullName = "Test User",
				PhoneNumber = "0123456789",
				Reason = "Muốn tham gia",
				FileUrl = "https://example.com/file.pdf"
			};

			UserRegister capturedRegister = null;

			_userRegisterMock
				.Setup(repo => repo.Insert(It.IsAny<UserRegister>()))
				.Callback<UserRegister>(ur => capturedRegister = ur)
				.Returns(Task.CompletedTask);

			
			var result = await _userRegisterService.CreateUserRegister(createRequest);

			
			Assert.True(result.Success);
			Assert.Equal("Create successfull", result.Message);
			Assert.NotNull(result.Data);
			Assert.Equal(createRequest.Email, result.Data.Email);
			Assert.Equal(UserRegisterEnum.REVIEWING.ToString(), result.Data.Status);

			
			_userRegisterMock.Verify(repo => repo.Insert(It.IsAny<UserRegister>()), Times.Once);

			
			Assert.NotNull(capturedRegister);
			Assert.Equal(createRequest.FullName, capturedRegister.FullName);
			Assert.Equal(createRequest.FileUrl, capturedRegister.FileUrl);
		}



		#endregion

		#region getUserRegister

		[Fact]
		public async Task GetAllUserRegisterAsync_Should_Return_Data_And_Total()
		{
			// Arrange
			var mockList = new List<UserRegister> { new() { Id = "1", FullName = "Test" } };

			_userRegisterMock
				.Setup(r => r.GetAllUserRegisterAsync(1, 10, null, null))
				.ReturnsAsync(mockList);

			_userRegisterMock
				.Setup(r => r.CountAllUserRegisterAsync())
				.ReturnsAsync(1);

			_mapperMock.Setup(m => m.Map<List<UserRegister>>(mockList))
				.Returns(new List<UserRegister> { new() { Id = "1", FullName = "Test" } });

			// Act
			var result = await _userRegisterService.GetAllUserRegisterAsync(1, 10, null, null);

			// Assert
			Assert.Single(result.Users);
			Assert.Equal(1, result.Total);
		}

		[Fact]
		public async Task GetAllUserRegisterAsyncByEmail_Should_Return_Filtered_Users()
		{
			var email = "user@example.com";
			var mockList = new List<UserRegister>
	{
		new() { Id = "1", Email = email }
	};

			_userRegisterMock
				.Setup(r => r.GetUserRegisterByEmail(email))
				.ReturnsAsync(mockList);

			_mapperMock.Setup(m => m.Map<List<UserRegister>>(mockList))
				.Returns(new List<UserRegister> { new() { Id = "1", Email = email } });

			var result = await _userRegisterService.GetAllUserRegisterAsyncByEmail(email);

			Assert.Single(result);
			Assert.Equal(email, result.First().Email);
		}

		[Fact]
		public async Task GetUserRegisterById_Should_Return_Error_When_Id_Is_Null()
		{
			var result = await _userRegisterService.GetUserRegisterById(null);

			Assert.False(result.Success);
			Assert.Equal("Id is not be null", result.Message);
		}

		[Fact]
		public async Task GetUserRegisterById_Should_Return_Error_When_Not_Found()
		{
			var id = "not-exist";

			_userRegisterMock
				.Setup(r => r.GetUserRegisterById(id))
				.ReturnsAsync((UserRegister)null);

			var result = await _userRegisterService.GetUserRegisterById(id);

			Assert.False(result.Success);
			Assert.Equal("Id does not exist", result.Message);
		}

		[Fact]
		public async Task GetUserRegisterById_Should_Return_Data_When_Found()
		{
			var id = "1";
			var userEntity = new UserRegister { Id = id, FullName = "Test" };
			var mappedUser = new UserRegister { Id = id, FullName = "Test" };

			_userRegisterMock
				.Setup(r => r.GetUserRegisterById(id))
				.ReturnsAsync(userEntity);

			_mapperMock.Setup(m => m.Map<UserRegister>(userEntity)).Returns(mappedUser);

			var result = await _userRegisterService.GetUserRegisterById(id);

			Assert.True(result.Success);
			Assert.Equal("Test", result.Data.FullName);
		}

		#endregion

	}
}
