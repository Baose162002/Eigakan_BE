using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
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
    public class AdMediaCountServiceTests
    {
        private readonly Mock<IAdMediaCountRepository> _mockMediaCountRepo = new();
        private readonly Mock<IAdMediaRepository> _mockMediaRepo = new();
        private readonly Mock<IAdPurchaseItemRepository> _mockPurchaseItemRepo = new();
        private readonly Mock<ILogger<AdMediaCountService>> _mockLogger = new();

        private readonly AdMediaCountService _service;

        public AdMediaCountServiceTests()
        {
            _service = new AdMediaCountService(
                _mockMediaCountRepo.Object,
                _mockMediaRepo.Object,
                _mockPurchaseItemRepo.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_Should_ReturnError_When_NoRemainingViews()
        {
            // Arrange
            string mediaId = "media-1";
            _mockPurchaseItemRepo.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync((AdPurchaseItems)null);

            // Act
            var result = await _service.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No remaining views available for this AdMedia.", result.Message);
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_Should_Update_ExistingCount_And_DecreaseView()
        {
            // Arrange
            string mediaId = "media-2";
            var purchaseItem = new AdPurchaseItems
            {
                Id = "purchase-1",
                AdMediaId = mediaId,
                RemainingViews = 5,
                Status = "ACTIVE"
            };
            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
            var existingCount = new AdMediaCount
            {
                Id = "count-1",
                AdMediaId = mediaId,
                ViewDate = today,
                ViewCount = 2
            };

            _mockPurchaseItemRepo.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync(purchaseItem);

            _mockMediaCountRepo.Setup(r => r.GetByMediaIdAndDate(mediaId, today))
                .ReturnsAsync(existingCount);

            _mockMediaCountRepo.Setup(r => r.Update(It.IsAny<AdMediaCount>()))
                .Returns(Task.CompletedTask);

            _mockPurchaseItemRepo.Setup(r => r.Update(It.IsAny<AdPurchaseItems>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(existingCount.Id, result.Data.Id);
            Assert.Equal(3, result.Data.ViewCount);
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_Should_Create_NewCount_And_DecreaseView()
        {
            // Arrange
            string mediaId = "media-3";
            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

            var purchaseItem = new AdPurchaseItems
            {
                Id = "purchase-2",
                AdMediaId = mediaId,
                RemainingViews = 3,
                Status = "ACTIVE"
            };

            _mockPurchaseItemRepo.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync(purchaseItem);

            _mockMediaCountRepo.Setup(r => r.GetByMediaIdAndDate(mediaId, today))
                .ReturnsAsync((AdMediaCount)null);

            _mockMediaCountRepo.Setup(r => r.Insert(It.IsAny<AdMediaCount>()))
                .Returns(Task.CompletedTask);

            _mockPurchaseItemRepo.Setup(r => r.Update(It.IsAny<AdPurchaseItems>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(mediaId, result.Data.AdMediaId);
            Assert.Equal(1, result.Data.ViewCount);
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_Should_SetInactive_When_RemainingViews_Zero()
        {
            // Arrange
            string mediaId = "media-4";
            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

            var purchaseItem = new AdPurchaseItems
            {
                Id = "purchase-3",
                AdMediaId = mediaId,
                RemainingViews = 1,
                Status = "ACTIVE"
            };

            var media = new AdMedia
            {
                Id = mediaId,
                status = "ACTIVE"
            };

            _mockPurchaseItemRepo.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync(purchaseItem);

            _mockMediaCountRepo.Setup(r => r.GetByMediaIdAndDate(mediaId, today))
                .ReturnsAsync((AdMediaCount)null);

            _mockMediaRepo.Setup(r => r.GetAdMediaById(mediaId)).ReturnsAsync(media);
            _mockMediaRepo.Setup(r => r.Update(media)).Returns(Task.CompletedTask);
            _mockMediaCountRepo.Setup(r => r.Insert(It.IsAny<AdMediaCount>())).Returns(Task.CompletedTask);
            _mockPurchaseItemRepo.Setup(r => r.Update(It.IsAny<AdPurchaseItems>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("INACTIVE", purchaseItem.Status);
            Assert.Equal("INACTIVE", media.status);
        }
        [Fact]
        public async Task StatisticAdMediaCount_ShouldReturnSuccess_WhenDataExists()
        {
            // Arrange
            var adMediaId = "admedia123";
            var data = new List<AdMediaCount>
        {
            new AdMediaCount { AdMediaId = adMediaId, ViewDate = new DateOnly(2024, 1, 1), ViewCount = 10 },
            new AdMediaCount { AdMediaId = adMediaId, ViewDate = new DateOnly(2024, 1, 1), ViewCount = 5 },
            new AdMediaCount { AdMediaId = adMediaId, ViewDate = new DateOnly(2024, 1, 2), ViewCount = 8 }
        };

            _mockMediaCountRepo.Setup(repo => repo.GetAllAdMediaCountByAdMediaId(adMediaId))
                .ReturnsAsync(data);

            // Act
            var result = await _service.StatisticAdMediaCount(adMediaId);

            // Assert
            Assert.NotNull(result);
            var successProp = result.GetType().GetProperty("Success");
            Assert.True((bool)successProp.GetValue(result));

            var dataProp = result.GetType().GetProperty("Data");
            var statistics = dataProp.GetValue(result) as IEnumerable<object>;

            Assert.Equal(2, statistics.Count()); // Two unique ViewDates
        }

        [Fact]
        public async Task StatisticAdMediaCount_ShouldReturnFailure_WhenNoData()
        {
            // Arrange
            var adMediaId = "noDataId";
            _mockMediaCountRepo.Setup(repo => repo.GetAllAdMediaCountByAdMediaId(adMediaId))
                .ReturnsAsync(new List<AdMediaCount>());

            // Act
            var result = await _service.StatisticAdMediaCount(adMediaId);

            // Assert
            var successProp = result.GetType().GetProperty("Success");
            Assert.False((bool)successProp.GetValue(result));

            var messageProp = result.GetType().GetProperty("Message");
            Assert.Equal("No statistics found", messageProp.GetValue(result));
        }

        [Fact]
        public async Task StatisticAdMediaCount_ShouldReturnFailure_WhenExceptionThrown()
        {
            // Arrange
            var adMediaId = "errorCase";
            _mockMediaCountRepo.Setup(repo => repo.GetAllAdMediaCountByAdMediaId(adMediaId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.StatisticAdMediaCount(adMediaId);

            // Assert
            var successProp = result.GetType().GetProperty("Success");
            Assert.False((bool)successProp.GetValue(result));

            var messageProp = result.GetType().GetProperty("Message");
            Assert.Equal("Database error", messageProp.GetValue(result));
        }
    }
}
