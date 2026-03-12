﻿﻿using AutoMapper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdMedia;
using Eigakan.Domain.Response.AdMediaCount;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class AdMediaCountServiceTest
    {
        private readonly Mock<IAdMediaCountRepository> _adMediaCountRepositoryMock;
        private readonly Mock<IAdMediaRepository> _adMediaRepositoryMock;
        private readonly Mock<IAdPurchaseItemRepository> _adPurchaseItemRepositoryMock;
        private readonly Mock<ILogger<AdMediaCountService>> _loggerMock;
        private readonly AdMediaCountService _adMediaCountService;

        public AdMediaCountServiceTest()
        {
            _adMediaCountRepositoryMock = new Mock<IAdMediaCountRepository>();
            _adMediaRepositoryMock = new Mock<IAdMediaRepository>();
            _adPurchaseItemRepositoryMock = new Mock<IAdPurchaseItemRepository>();
            _loggerMock = new Mock<ILogger<AdMediaCountService>>();

            _adMediaCountService = new AdMediaCountService(
                _adMediaCountRepositoryMock.Object,
                _adMediaRepositoryMock.Object,
                _adPurchaseItemRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task IncreaseAdMediaCount_ShouldReturnError_WhenAdMediaIdIsEmpty()
        {
            // Arrange
            var request = new AdClickCountCreateRequest
            {
                AdMediaId = "",
                MovieId = "movie-1"
            };

            // Act
            var result = await _adMediaCountService.IncreaseAdMediaCount(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("AdMediaId is required", result.Message);
        }

        [Fact]
        public async Task IncreaseAdMediaCount_ShouldReturnError_WhenMovieIdIsEmpty()
        {
            // Arrange
            var request = new AdClickCountCreateRequest
            {
                AdMediaId = "ad-1",
                MovieId = ""
            };

            // Act
            var result = await _adMediaCountService.IncreaseAdMediaCount(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("MovieId is required", result.Message);
        }

        [Fact]
        public async Task IncreaseAdMediaCount_ShouldReturnError_WhenAdMediaNotFound()
        {
            // Arrange
            var request = new AdClickCountCreateRequest
            {
                AdMediaId = "non-existent-ad",
                MovieId = "movie-1"
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(request.AdMediaId)).ReturnsAsync((AdMedia)null);

            // Act
            var result = await _adMediaCountService.IncreaseAdMediaCount(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"AdMedia with ID {request.AdMediaId} not found", result.Message);
        }

        [Fact]
        public async Task IncreaseAdMediaCount_ShouldUpdateExistingCount_WhenCountExists()
        {
            // Arrange
            var request = new AdClickCountCreateRequest
            {
                AdMediaId = "ad-1",
                MovieId = "movie-1"
            };

            var adMedia = new AdMedia { Id = "ad-1" };
            var dateOnly = DateOnly.FromDateTime(DateTime.Now);
            var existingCount = new AdMediaCount
            {
                Id = "count-1",
                ViewCount = 5,
                ViewDate = dateOnly
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(request.AdMediaId)).ReturnsAsync(adMedia);
            _adMediaCountRepositoryMock.Setup(r => r.CheckCountByAdMediaDate(request.AdMediaId, request.MovieId, It.IsAny<DateOnly>()))
                .ReturnsAsync(existingCount);
            _adMediaCountRepositoryMock.Setup(r => r.UpdateViewCount(request.AdMediaId, request.MovieId, It.IsAny<DateOnly>()))
                .ReturnsAsync(existingCount);

            // Act
            var result = await _adMediaCountService.IncreaseAdMediaCount(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(existingCount, result.Data);
            Assert.Equal(6, existingCount.ViewCount); // ViewCount should be incremented
        }

        [Fact]
        public async Task IncreaseAdMediaCount_ShouldCreateNewCount_WhenCountDoesNotExist()
        {
            // Arrange
            var request = new AdClickCountCreateRequest
            {
                AdMediaId = "ad-1",
                MovieId = "movie-1"
            };

            var adMedia = new AdMedia { Id = "ad-1" };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(request.AdMediaId)).ReturnsAsync(adMedia);
            _adMediaCountRepositoryMock.Setup(r => r.CheckCountByAdMediaDate(request.AdMediaId, request.MovieId, It.IsAny<DateOnly>()))
                .ReturnsAsync((AdMediaCount)null);
            _adMediaCountRepositoryMock.Setup(r => r.InsertWithAdMedia(It.IsAny<AdMediaCount>(), request.AdMediaId, request.MovieId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adMediaCountService.IncreaseAdMediaCount(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.ViewCount);
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_ShouldReturnError_WhenNoRemainingViews()
        {
            // Arrange
            var mediaId = "ad-1";

            _adPurchaseItemRepositoryMock.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync((AdPurchaseItems)null);

            // Act
            var result = await _adMediaCountService.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No remaining views available for this AdMedia.", result.Message);
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_ShouldUpdateExistingCount_WhenCountExists()
        {
            // Arrange
            var mediaId = "ad-1";
            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
            
            var purchaseItem = new AdPurchaseItems
            {
                Id = "purchase-1",
                RemainingViews = 10,
                Status = "ACTIVE"
            };

            var existingCount = new AdMediaCount
            {
                Id = "count-1",
                AdMediaId = mediaId,
                ViewDate = today,
                ViewCount = 5
            };

            _adPurchaseItemRepositoryMock.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync(purchaseItem);
            _adMediaCountRepositoryMock.Setup(r => r.GetByMediaIdAndDate(mediaId, It.IsAny<DateOnly>()))
                .ReturnsAsync(existingCount);
            _adMediaCountRepositoryMock.Setup(r => r.Update(It.IsAny<AdMediaCount>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adMediaCountService.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(6, existingCount.ViewCount);
            Assert.Equal(9, purchaseItem.RemainingViews);
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_ShouldCreateNewCount_WhenCountDoesNotExist()
        {
            // Arrange
            var mediaId = "ad-1";
            
            var purchaseItem = new AdPurchaseItems
            {
                Id = "purchase-1",
                RemainingViews = 10,
                Status = "ACTIVE"
            };

            _adPurchaseItemRepositoryMock.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync(purchaseItem);
            _adMediaCountRepositoryMock.Setup(r => r.GetByMediaIdAndDate(mediaId, It.IsAny<DateOnly>()))
                .ReturnsAsync((AdMediaCount)null);
            _adMediaCountRepositoryMock.Setup(r => r.Insert(It.IsAny<AdMediaCount>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adMediaCountService.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(9, purchaseItem.RemainingViews);
        }

        [Fact]
        public async Task CreateCountAdMediaAsync_ShouldDeactivateItem_WhenRemainingViewsReachesZero()
        {
            // Arrange
            var mediaId = "ad-1";
            
            var purchaseItem = new AdPurchaseItems
            {
                Id = "purchase-1",
                RemainingViews = 1,
                Status = "ACTIVE"
            };

            var adMedia = new AdMedia
            {
                Id = mediaId,
                status = "ACTIVE"
            };

            _adPurchaseItemRepositoryMock.Setup(r => r.GetByMediaIdAndHasRemainingViews(mediaId))
                .ReturnsAsync(purchaseItem);
            _adMediaCountRepositoryMock.Setup(r => r.GetByMediaIdAndDate(mediaId, It.IsAny<DateOnly>()))
                .ReturnsAsync((AdMediaCount)null);
            _adMediaCountRepositoryMock.Setup(r => r.Insert(It.IsAny<AdMediaCount>()))
                .Returns(Task.CompletedTask);
            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(mediaId))
                .ReturnsAsync(adMedia);
            _adPurchaseItemRepositoryMock.Setup(r => r.Update(It.IsAny<AdPurchaseItems>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adMediaCountService.CreateCountAdMediaAsync(mediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(0, purchaseItem.RemainingViews);
            Assert.Equal("INACTIVE", purchaseItem.Status);
        }

        [Fact]
        public async Task GetAdMediaCountByAdMediaId_ShouldReturnSuccess_WhenAdMediaCountExists()
        {
            // Arrange
            var adMediaId = "ad-1";
            var adMediaCount = new AdMediaCount
            {
                Id = "count-1",
                AdMediaId = adMediaId,
                ViewCount = 10
            };

            _adMediaCountRepositoryMock.Setup(r => r.GetAdMediaCountByAdMediaId(adMediaId))
                .ReturnsAsync(adMediaCount);

            // Act
            var result = await _adMediaCountService.GetAdMediaCountByAdMediaId(adMediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(adMediaCount, result.Data);
        }

        [Fact]
        public async Task GetAdMediaCountByAdMediaId_ShouldReturnError_WhenAdMediaCountNotFound()
        {
            // Arrange
            var adMediaId = "non-existent-ad";

            _adMediaCountRepositoryMock.Setup(r => r.GetAdMediaCountByAdMediaId(adMediaId))
                .ReturnsAsync((AdMediaCount)null);

            // Act
            var result = await _adMediaCountService.GetAdMediaCountByAdMediaId(adMediaId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("AdMediaCount not found", result.Message);
        }

        [Fact]
        public async Task StatisticAdMediaCount_ShouldReturnStatistics_WhenAdMediaExists()
        {
            // Arrange
            var adMediaId = "ad-1";
            var adMediaCounts = new List<AdMediaCount>
            {
                new AdMediaCount { Id = "count-1", AdMediaId = adMediaId, ViewCount = 5, ViewDate = new DateOnly(2023, 1, 1) },
                new AdMediaCount { Id = "count-2", AdMediaId = adMediaId, ViewCount = 10, ViewDate = new DateOnly(2023, 1, 2) }
            };

            _adMediaCountRepositoryMock.Setup(r => r.GetAllAdMediaCountByAdMediaId(adMediaId))
                .ReturnsAsync(adMediaCounts);

            // Act
            var result = await _adMediaCountService.StatisticAdMediaCount(adMediaId);

            // Assert
            Assert.NotNull(result);
            // Additional assertions would depend on the actual implementation of StatisticAdMediaCount
        }
    }
}
