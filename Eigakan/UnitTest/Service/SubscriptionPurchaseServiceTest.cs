using AutoMapper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.SubscriptionPurchaseResponse;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class SubscriptionPurchaseServiceTest
    {
        private readonly Mock<ISubscriptionPurchaseRepository> _subscriptionPurchaseRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SubscriptionPurchaseService>> _loggerMock;
        private readonly SubscriptionPurchaseService _service;

        public SubscriptionPurchaseServiceTest()
        {
            _subscriptionPurchaseRepoMock = new Mock<ISubscriptionPurchaseRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SubscriptionPurchaseService>>();
            _service = new SubscriptionPurchaseService(_subscriptionPurchaseRepoMock.Object, _userRepoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }
        #region  SavePurchaseAsync
        [Fact]
        public async Task SavePurchaseAsync_ShouldReturnSuccess_WhenValidPurchase()
        {
            var purchase = new SubscriptionPurchase { Id = "1", TotalPrice = 100 };
            var result = await _service.SavePurchaseAsync(purchase);
            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task SavePurchaseAsync_ShouldReturnFail_WhenNullPurchase()
        {
            var result = await _service.SavePurchaseAsync(null);
            Assert.False(result.Success);
        }
        #endregion

        #region  UpdateStatusUserSubscriptionPurchase
        [Fact]
        public async Task UpdateStatusUserSubscriptionPurchase_ShouldUpdateRole_WhenValidUser()
        {
            var user = new User { Id = "user1", RoleId = "old" };
            _userRepoMock.Setup(r => r.GetUserById("user1")).ReturnsAsync(user);

            var result = await _service.UpdateStatusUserSubscriptionPurchase("user1");

            Assert.True(result.Success);
            Assert.Equal("33AAA70C", user.RoleId);
        }

        [Fact]
        public async Task UpdateStatusUserSubscriptionPurchase_ShouldReturnFail_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetUserById("user1")).ReturnsAsync((User)null);

            var result = await _service.UpdateStatusUserSubscriptionPurchase("user1");

            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdateExpiredSubscriptions_ShouldUpdateStatusAndUserRole_WhenExpired()
        {
            // Arrange
            var vietnamTime = DateTime.UtcNow.AddHours(-1); // Simulate expired
            var expiredSub = new SubscriptionPurchase { Id = "1", UserId = "user1", ExpiredDate = vietnamTime.AddDays(-1), Status = "Active" };
            var user = new User { Id = "user1", RoleId = "33AAA70C" };

            _subscriptionPurchaseRepoMock.Setup(r => r.GetExpiredSubscriptions()).ReturnsAsync(new List<SubscriptionPurchase> { expiredSub });
            _userRepoMock.Setup(r => r.GetUserById("user1")).ReturnsAsync(user);
            _subscriptionPurchaseRepoMock.Setup(r => r.GetLatestUserSubscription("user1")).ReturnsAsync((SubscriptionPurchase)null);

            // Act
            await _service.UpdateExpiredSubscriptions();

            // Assert
            _subscriptionPurchaseRepoMock.Verify(r => r.Update(It.Is<SubscriptionPurchase>(s => s.Status == "Expired")), Times.Once);
            _userRepoMock.Verify(r => r.Update(It.Is<User>(u => u.RoleId == "43AAA70C")), Times.Once);
        }
        [Fact]
        public async Task UpdateExpiredSubscriptions_ShouldNotUpdate_WhenNotExpired()
        {
            var now = DateTime.UtcNow;
            var futureSub = new SubscriptionPurchase { Id = "1", UserId = "user1", ExpiredDate = now.AddDays(2), Status = "Active" };

            _subscriptionPurchaseRepoMock.Setup(r => r.GetExpiredSubscriptions()).ReturnsAsync(new List<SubscriptionPurchase> { futureSub });

            await _service.UpdateExpiredSubscriptions();

            _subscriptionPurchaseRepoMock.Verify(r => r.Update(It.IsAny<SubscriptionPurchase>()), Times.Never);
            _userRepoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region  GetLatestUserSubscription
        [Fact]
        public async Task GetLatestUserSubscription_ShouldReturnSubscription_WhenExists()
        {
            var sub = new SubscriptionPurchase { Id = "1" };
            _subscriptionPurchaseRepoMock.Setup(r => r.GetLatestUserSubscription("user1")).ReturnsAsync(sub);

            var result = await _service.GetLatestUserSubscription("user1");

            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetAllSubscriptionPurchaseUser_ShouldReturnSuccess_WhenDataFound()
        {
            var user = new User { Id = "user1" };
            var list = new List<SubscriptionPurchase> { new SubscriptionPurchase { Id = "1" } };
            _userRepoMock.Setup(r => r.GetUserById("user1")).ReturnsAsync(user);
            _subscriptionPurchaseRepoMock.Setup(r => r.GetSubscriptionPurchaseUserById("user1", 1, 10)).ReturnsAsync(list);
            _subscriptionPurchaseRepoMock.Setup(r => r.CountAllSubscriptionPackageAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<List<SubscriptionPurchaseGetAllResponse>>(list))
                .Returns(new List<SubscriptionPurchaseGetAllResponse> { new SubscriptionPurchaseGetAllResponse() });

            var result = await _service.GetAllSubscriptionPurchaseUser("user1", 1, 10);

            Assert.True(result.Success);
            Assert.Single(result.Data.SubscriptionPurchases);
        }
        #endregion
        #region GetAllSubscriptionPurchaseAsync
        [Fact]
        public async Task GetAllSubscriptionPurchaseAsync_ShouldReturnCorrectStats_WhenDataExists()
        {
            var data = new List<SubscriptionPurchase>
    {
        new SubscriptionPurchase { Id = "1", Status = "Active", TotalPrice = 100 },
        new SubscriptionPurchase { Id = "2", Status = "Expired", TotalPrice = 200 }
    };

            _subscriptionPurchaseRepoMock.Setup(r => r.GetAllSubscriptionPurchase(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<decimal?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(data);

            _subscriptionPurchaseRepoMock.Setup(r => r.GetAllSubscriptionPurchaseNoPaging()).ReturnsAsync(data);
            _subscriptionPurchaseRepoMock.Setup(r => r.CountAllSubscriptionPackageAsync()).ReturnsAsync(2);

            _mapperMock.Setup(m => m.Map<List<SubscriptionPurchaseGetAllResponse>>(It.IsAny<List<SubscriptionPurchase>>()))
                .Returns(new List<SubscriptionPurchaseGetAllResponse> { new(), new() });

            var result = await _service.GetAllSubscriptionPurchaseAsync(1, 10, null, null, null, null, null, null, null, null);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Total);
            Assert.Equal(1, result.Data.ActiveSubscriptionCount);
            Assert.Equal(300, result.Data.totalEarnings);
        }
        #endregion
    }
}
