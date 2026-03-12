using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.UserWallet;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class UserWalletServiceTest
    {
        private readonly Mock<IUserWalletRepository> _userWalletRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserWalletService _service;

        public UserWalletServiceTest()
        {
            _userWalletRepositoryMock = new Mock<IUserWalletRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _mapperMock = new Mock<IMapper>();

            _service = new UserWalletService(
                _userWalletRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetUserWalletById_ShouldReturnSuccess_WhenUserWalletExists()
        {
            // Arrange
            var userId = "user123";
            var wallet = new UserWallet { Id = "wallet1", UserId = userId, Balance = 100 };
            var response = new UserWalletGetAllResponse();

            var claims = new List<Claim> { new Claim(MySetting.CLAIM_USERID, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var user = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = user };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
            _userWalletRepositoryMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync(wallet);
            _mapperMock.Setup(m => m.Map<UserWalletGetAllResponse>(wallet)).Returns(response);

            // Act
            var result = await _service.GetUserWalletById();

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetUserWalletById_ShouldReturnFailure_WhenUserNotAuthenticated()
        {
            // Arrange
            var context = new DefaultHttpContext(); // No claims
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = await _service.GetUserWalletById();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not authenticated.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUserWalletById_ShouldReturnFailure_WhenWalletNotFound()
        {
            // Arrange
            var userId = "user123";
            var claims = new List<Claim> { new Claim(MySetting.CLAIM_USERID, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var user = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = user };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
            _userWalletRepositoryMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync((UserWallet)null);

            // Act
            var result = await _service.GetUserWalletById();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User Wallet not found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUserWalletById_ShouldReturnFailure_WhenExceptionThrown()
        {
            // Arrange
            var userId = "user123";
            var claims = new List<Claim> { new Claim(MySetting.CLAIM_USERID, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var user = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = user };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
            _userWalletRepositoryMock
                .Setup(r => r.GetUserWalletById(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetUserWalletById();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.Message);
            Assert.Null(result.Data);
        }
    }
}
