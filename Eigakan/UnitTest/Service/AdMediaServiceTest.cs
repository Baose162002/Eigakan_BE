﻿﻿using AutoMapper;
using Discord;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdMedia;
using Eigakan.Domain.Response.AdMediaResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class AdMediaServiceTest
    {
        private readonly Mock<IAdMediaRepository> _adMediaRepositoryMock;
        private readonly Mock<IAdMediaCountRepository> _adMediaCountRepositoryMock;
        private readonly Mock<IAdPurchaseItemRepository> _adPurchaseItemRepositoryMock;
        private readonly Mock<IAdPurchaseTransactionRepository> _adPurchaseTransactionRepositoryMock;
        private readonly Mock<IMoviesRepository> _moviesRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<Logger> _loggerMock;
        private readonly Mock<IAdPackageRepository> _adPackageRepositoryMock;
        private readonly Mock<IUserWalletRepository> _userWalletRepositoryMock;
        private readonly Mock<IWalletTransactionRepository> _walletTransactionRepositoryMock;

        private readonly AdMediaService _adMediaService;

        public AdMediaServiceTest()
        {
            _adMediaRepositoryMock = new Mock<IAdMediaRepository>();
            _adMediaCountRepositoryMock = new Mock<IAdMediaCountRepository>();
            _adPurchaseItemRepositoryMock = new Mock<IAdPurchaseItemRepository>();
            _adPurchaseTransactionRepositoryMock = new Mock<IAdPurchaseTransactionRepository>();
            _moviesRepositoryMock = new Mock<IMoviesRepository>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _adPackageRepositoryMock = new Mock<IAdPackageRepository>();
            _userWalletRepositoryMock = new Mock<IUserWalletRepository>();
            _walletTransactionRepositoryMock = new Mock<IWalletTransactionRepository>();

            _loggerMock = new Mock<Logger>(MockBehavior.Loose, new object[] { null });
            _loggerMock.Setup(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _adMediaService = new AdMediaService(
                _adMediaRepositoryMock.Object,
                _loggerMock.Object,
                _adMediaCountRepositoryMock.Object,
                _adPurchaseTransactionRepositoryMock.Object,
                _adPurchaseItemRepositoryMock.Object,
                _moviesRepositoryMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object,
                _adPackageRepositoryMock.Object,
                _userWalletRepositoryMock.Object,
                _walletTransactionRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetAllListAdMedia_ShouldReturnSuccess_WhenRepositoryReturnsData()
        {
            // Arrange
            var status = "ACTIVE";
            var page = 1;
            var pageSize = 10;
            var adMediaList = new List<AdMedia>
            {
                new AdMedia { Id = "ad1", Content = "Test Content 1", status = "ACTIVE" },
                new AdMedia { Id = "ad2", Content = "Test Content 2", status = "ACTIVE" }
            };

            _adMediaRepositoryMock.Setup(r => r.GetList(status, page, pageSize))
                .ReturnsAsync(adMediaList);

            // Act
            var result = await _adMediaService.GetAllListAdMedia(status, page, pageSize);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(adMediaList, result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetAllListAdMedia_ShouldReturnError_WhenExceptionOccurs()
        {
            // Arrange
            var status = "ACTIVE";
            var page = 1;
            var pageSize = 10;
            var exceptionMessage = "Database connection error";

            _adMediaRepositoryMock.Setup(r => r.GetList(status, page, pageSize))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _adMediaService.GetAllListAdMedia(status, page, pageSize);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.Message);
        }

        [Fact]
        public async Task GetById_ShouldReturnSuccess_WhenAdMediaExists()
        {
            // Arrange
            var adMediaId = "ad1";
            var adMedia = new AdMedia
            {
                Id = adMediaId,
                Content = "Test Content",
                status = "ACTIVE"
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(adMediaId))
                .ReturnsAsync(adMedia);

            // Act
            var result = await _adMediaService.GetById(adMediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Success.", result.Message);
            Assert.Equal(adMedia, result.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnError_WhenAdMediaNotFound()
        {
            // Arrange
            var adMediaId = "nonexistent";

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(adMediaId))
                .ReturnsAsync((AdMedia)null);

            // Act
            var result = await _adMediaService.GetById(adMediaId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("AdMedia not found.", result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenAdMediaDeleted()
        {
            // Arrange
            var adMediaId = "ad1";
            var adMedia = new AdMedia
            {
                Id = adMediaId,
                Content = "Test Content",
                status = "ACTIVE"
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(adMediaId))
                .ReturnsAsync(adMedia);
            _adMediaRepositoryMock.Setup(r => r.DeleteAdMediaAsync(adMediaId))
                .ReturnsAsync(true);

            // Act
            var result = await _adMediaService.Delete(adMediaId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete success.", result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnError_WhenAdMediaNotFound()
        {
            // Arrange
            var adMediaId = "nonexistent";

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(adMediaId))
                .ReturnsAsync((AdMedia)null);

            // Act
            var result = await _adMediaService.Delete(adMediaId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("AdMedia not found.", result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnError_WhenDeleteFails()
        {
            // Arrange
            var adMediaId = "ad1";
            var adMedia = new AdMedia
            {
                Id = adMediaId,
                Content = "Test Content",
                status = "ACTIVE"
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(adMediaId))
                .ReturnsAsync(adMedia);
            _adMediaRepositoryMock.Setup(r => r.DeleteAdMediaAsync(adMediaId))
                .ReturnsAsync(false);

            // Act
            var result = await _adMediaService.Delete(adMediaId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Delete fail.", result.Message);
        }

        [Fact]
        public async Task AdMediaApprovedStatus_ShouldReturnSuccess_WhenAdMediaExists()
        {
            // Arrange
            var request = new AdMediaApprovedStatus { Id = "ad1" };
            var adMedia = new AdMedia
            {
                Id = request.Id,
                Content = "Test Content",
                status = "PENDING"
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(request.Id))
                .ReturnsAsync(adMedia);
            _adMediaRepositoryMock.Setup(r => r.Update(It.IsAny<AdMedia>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adMediaService.AdMediaApprovedStatus(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(adMedia, result.Data);
            Assert.Equal(AdStatusEnum.ACTIVE.ToString(), adMedia.status);
            Assert.Null(adMedia.ReasonForRejection);
            Assert.NotNull(adMedia.ApprovedDate);
        }

        [Fact]
        public async Task AdMediaApprovedStatus_ShouldReturnError_WhenAdMediaNotFound()
        {
            // Arrange
            var request = new AdMediaApprovedStatus { Id = "nonexistent" };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(request.Id))
                .ReturnsAsync((AdMedia)null);

            // Act
            var result = await _adMediaService.AdMediaApprovedStatus(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("AdMedia not found.", result.Message);
        }

        [Fact]
        public async Task AdMediaRejectedStatus_ShouldReturnSuccess_WhenAdMediaExists()
        {
            // Arrange
            var request = new AdMediaRejectedRequest
            {
                Id = "ad1",
                ReasonForRejection = "Content not appropriate"
            };
            var adMedia = new AdMedia
            {
                Id = request.Id,
                Content = "Test Content",
                status = "PENDING"
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(request.Id))
                .ReturnsAsync(adMedia);
            _adMediaRepositoryMock.Setup(r => r.Update(It.IsAny<AdMedia>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adMediaService.AdMediaRejectedStatus(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(adMedia, result.Data);
            Assert.Equal(AdStatusEnum.REJECTED.ToString(), adMedia.status);
            Assert.Equal(request.ReasonForRejection, adMedia.ReasonForRejection);
            Assert.Null(adMedia.ApprovedDate);
        }

        [Fact]
        public async Task AdMediaRejectedStatus_ShouldReturnError_WhenAdMediaNotFound()
        {
            // Arrange
            var request = new AdMediaRejectedRequest
            {
                Id = "nonexistent",
                ReasonForRejection = "Content not appropriate"
            };

            _adMediaRepositoryMock.Setup(r => r.GetAdMediaById(request.Id))
                .ReturnsAsync((AdMedia)null);

            // Act
            var result = await _adMediaService.AdMediaRejectedStatus(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("AdMedia not found.", result.Message);
        }



        [Fact]
        public async Task GetAdMediaWithPositionsAsync_ShouldReturnEmptyList_WhenMovieNotFound()
        {
            // Arrange
            var movieId = "movie1";

            _moviesRepositoryMock.Setup(r => r.GetMovieById(movieId))
                .ReturnsAsync((Movie)null);

            // Act
            var result = await _adMediaService.GetAdMediaWithPositionsAsync(movieId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAdMediaWithPositionsAsync_ShouldReturnEmptyList_WhenMovieDurationTooShort()
        {
            // Arrange
            var movieId = "movie1";
            var movie = new Movie
            {
                Id = movieId,
                Title = "Short Movie",
                Duration = 1 // 1 minute, too short
            };

            _moviesRepositoryMock.Setup(r => r.GetMovieById(movieId))
                .ReturnsAsync(movie);

            // Act
            var result = await _adMediaService.GetAdMediaWithPositionsAsync(movieId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMediaByUserIdAsync_ShouldReturnError_WhenUserNotAuthenticated()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            _httpContextAccessorMock.Setup(h => h.HttpContext.User.FindFirst(MySetting.CLAIM_USERID))
                .Returns((Claim)null);

            // Act
            var result = await _adMediaService.GetMediaByUserIdAsync(page, pageSize);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not authenticated", result.Message);
        }

        [Fact]
        public async Task GetMediaByUserIdAsync_ShouldReturnSuccess_WhenNoTransactionsFound()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var userId = "user1";

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(MySetting.CLAIM_USERID, userId)
            }));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _adPurchaseTransactionRepositoryMock.Setup(r => r.GetAdPurchaseTransactionByUserId(userId))
                .ReturnsAsync(new List<AdPurchaseTransaction>());

            // Act
            var result = await _adMediaService.GetMediaByUserIdAsync(page, pageSize);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("No ad transactions found.", result.Message);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetMediaByUserIdAsync_ShouldReturnSuccess_WithMediaList()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var userId = "user1";

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(MySetting.CLAIM_USERID, userId)
            }));

            var transactions = new List<AdPurchaseTransaction>
            {
                new AdPurchaseTransaction { Id = "trans1", UserId = userId }
            };

            var items = new List<AdPurchaseItems>
            {
                new AdPurchaseItems { Id = "item1", AdMediaId = "ad1", AdPurchaseTransactionId = "trans1" }
            };

            var mediaList = new List<AdMedia>
            {
                new AdMedia { Id = "ad1", Content = "Test Content", status = "ACTIVE" }
            };

            var mediaResponses = new List<AdMediaGetAllResponse>
            {
                new AdMediaGetAllResponse { Id = "ad1", Content = "Test Content", status = "ACTIVE" }
            };

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _adPurchaseTransactionRepositoryMock.Setup(r => r.GetAdPurchaseTransactionByUserId(userId))
                .ReturnsAsync(transactions);
            _adPurchaseItemRepositoryMock.Setup(r => r.GetItemsByTransactionIdAsync("trans1"))
                .ReturnsAsync(items);
            _adMediaRepositoryMock.Setup(r => r.GetListMediaByUserId("ad1", 1, 1))
                .ReturnsAsync(mediaList);

            // Act
            var result = await _adMediaService.GetMediaByUserIdAsync(page, pageSize);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Media list retrieved successfully.", result.Message);
            Assert.Single(result.Data);
        }
    }
}
