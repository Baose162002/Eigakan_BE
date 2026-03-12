using AutoMapper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.News;
using Eigakan.Domain.Response.News;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class NewsServiceTest
    {
        private readonly Mock<INewsRepository> _newsRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<NewsService>> _loggerMock;
        private readonly NewsService _newsService;

        public NewsServiceTest()
        {
            _newsRepositoryMock = new Mock<INewsRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<NewsService>>();

            _newsService = new NewsService(
                _newsRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _userRepositoryMock.Object
            );
        }

        #region GetList

        [Fact]
        public async Task GetList_Should_Return_Success_With_Data()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News { Id = "news1", Title = "News 1" },
                new News { Id = "news2", Title = "News 2" }
            };

            var newsResponseList = new List<NewsResponse>
            {
                new NewsResponse { Id = "news1", Title = "News 1" },
                new NewsResponse { Id = "news2", Title = "News 2" }
            };

            _newsRepositoryMock.Setup(r => r.GetList()).ReturnsAsync(newsList);
            _mapperMock.Setup(m => m.Map<List<NewsResponse>>(newsList)).Returns(newsResponseList);

            // Act
            var result = await _newsService.GetList();

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Get list successfully", result.Message);
            Assert.Equal(newsResponseList, result.Data);
        }

        [Fact]
        public async Task GetList_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            _newsRepositoryMock.Setup(r => r.GetList()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _newsService.GetList();

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to get news list", result.Message);
        }

        #endregion

        #region GetNewsById

        [Fact]
        public async Task GetNewsById_Should_Return_Success_When_News_Exists()
        {
            // Arrange
            var newsId = "news123";
            var news = new News { Id = newsId, Title = "Test News" };
            var newsResponse = new NewsResponse { Id = newsId, Title = "Test News" };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync(news);
            _mapperMock.Setup(m => m.Map<NewsResponse>(news)).Returns(newsResponse);

            // Act
            var result = await _newsService.GetNewsById(newsId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Get news successfully", result.Message);
            Assert.Equal(newsResponse, result.Data);
        }

        [Fact]
        public async Task GetNewsById_Should_Return_Failure_When_Id_Is_Empty()
        {
            // Arrange
            string newsId = string.Empty;

            // Act
            var result = await _newsService.GetNewsById(newsId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("News ID cannot be empty", result.Message);
        }

        [Fact]
        public async Task GetNewsById_Should_Return_Failure_When_News_Not_Found()
        {
            // Arrange
            var newsId = "nonexistent";
            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync((News)null);

            // Act
            var result = await _newsService.GetNewsById(newsId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("News not found", result.Message);
        }

        [Fact]
        public async Task GetNewsById_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            var newsId = "news123";
            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _newsService.GetNewsById(newsId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to get news", result.Message);
        }

        #endregion

        #region CreateNews

        [Fact]
        public async Task CreateNews_Should_Return_Success_When_Request_Is_Valid()
        {
            // Arrange
            var userId = "user123";
            var user = new User { Id = userId };
            var request = new CreateNewsRequest
            {
                Title = "Test News",
                Content = "Test Content",
                UserId = userId
            };

            var createdNews = new News
            {
                Id = "news123",
                Title = request.Title,
                Content = request.Content,
                UserId = userId
            };

            var newsResponse = new NewsResponse
            {
                Id = "news123",
                Title = request.Title,
                Content = request.Content
            };

            _userRepositoryMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
            _newsRepositoryMock.Setup(r => r.Insert(It.IsAny<News>())).Returns(Task.CompletedTask);
            _newsRepositoryMock.Setup(r => r.GetNewsById(It.IsAny<string>())).ReturnsAsync(createdNews);
            _mapperMock.Setup(m => m.Map<NewsResponse>(createdNews)).Returns(newsResponse);

            // Act
            var result = await _newsService.CreateNews(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Create news successfully", result.Message);
            Assert.Equal(newsResponse, result.Data);
        }

        [Fact]
        public async Task CreateNews_Should_Return_Failure_When_Request_Is_Null()
        {
            // Act
            var result = await _newsService.CreateNews(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Request cannot be null", result.Message);
        }

        [Fact]
        public async Task CreateNews_Should_Return_Failure_When_Title_Is_Empty()
        {
            // Arrange
            var request = new CreateNewsRequest
            {
                Title = "",
                Content = "Test Content"
            };

            // Act
            var result = await _newsService.CreateNews(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Title is required", result.Message);
        }

        [Fact]
        public async Task CreateNews_Should_Return_Failure_When_Content_Is_Empty()
        {
            // Arrange
            var request = new CreateNewsRequest
            {
                Title = "Test Title",
                Content = ""
            };

            // Act
            var result = await _newsService.CreateNews(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Content is required", result.Message);
        }

        [Fact]
        public async Task CreateNews_Should_Return_Failure_When_User_Not_Found()
        {
            // Arrange
            var userId = "nonexistent";
            var request = new CreateNewsRequest
            {
                Title = "Test News",
                Content = "Test Content",
                UserId = userId
            };

            _userRepositoryMock.Setup(r => r.GetUserById(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _newsService.CreateNews(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid UserId. User not found.", result.Message);
        }

        [Fact]
        public async Task CreateNews_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            var userId = "user123";
            var user = new User { Id = userId };
            var request = new CreateNewsRequest
            {
                Title = "Test News",
                Content = "Test Content",
                UserId = userId
            };

            _userRepositoryMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
            _newsRepositoryMock.Setup(r => r.Insert(It.IsAny<News>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _newsService.CreateNews(request);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to create news", result.Message);
        }

        #endregion

        #region UpdateNews

        [Fact]
        public async Task UpdateNews_Should_Return_Success_When_Request_Is_Valid()
        {
            // Arrange
            var newsId = "news123";
            var existingNews = new News
            {
                Id = newsId,
                Title = "Old Title",
                Content = "Old Content",
                Status = "Active"
            };

            var updateRequest = new UpdateNewsRequest
            {
                Title = "New Title",
                Content = "New Content"
            };

            var newsResponse = new NewsResponse
            {
                Id = newsId,
                Title = "New Title",
                Content = "New Content"
            };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync(existingNews);
            _newsRepositoryMock.Setup(r => r.Update(It.IsAny<News>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<NewsResponse>(It.IsAny<News>())).Returns(newsResponse);

            // Act
            var result = await _newsService.UpdateNews(newsId, updateRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Update news successfully", result.Message);
            Assert.Equal(newsResponse, result.Data);
        }

        [Fact]
        public async Task UpdateNews_Should_Return_Failure_When_Id_Is_Empty()
        {
            // Arrange
            string newsId = string.Empty;
            var updateRequest = new UpdateNewsRequest { Title = "New Title" };

            // Act
            var result = await _newsService.UpdateNews(newsId, updateRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("News ID cannot be empty", result.Message);
        }

        [Fact]
        public async Task UpdateNews_Should_Return_Failure_When_Request_Is_Null()
        {
            // Arrange
            var newsId = "news123";

            // Act
            var result = await _newsService.UpdateNews(newsId, null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Request cannot be null", result.Message);
        }

        [Fact]
        public async Task UpdateNews_Should_Return_Failure_When_News_Not_Found()
        {
            // Arrange
            var newsId = "nonexistent";
            var updateRequest = new UpdateNewsRequest { Title = "New Title" };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync((News)null);

            // Act
            var result = await _newsService.UpdateNews(newsId, updateRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("News not found", result.Message);
        }

        [Fact]
        public async Task UpdateNews_Should_Return_Failure_When_Status_Is_Invalid()
        {
            // Arrange
            var newsId = "news123";
            var existingNews = new News
            {
                Id = newsId,
                Title = "Old Title",
                Status = "Active"
            };

            var updateRequest = new UpdateNewsRequest
            {
                Status = "InvalidStatus"
            };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync(existingNews);

            // Act
            var result = await _newsService.UpdateNews(newsId, updateRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Invalid status value", result.Message);
        }

        [Fact]
        public async Task UpdateNews_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            var newsId = "news123";
            var existingNews = new News
            {
                Id = newsId,
                Title = "Old Title"
            };

            var updateRequest = new UpdateNewsRequest { Title = "New Title" };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync(existingNews);
            _newsRepositoryMock.Setup(r => r.Update(It.IsAny<News>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _newsService.UpdateNews(newsId, updateRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to update news", result.Message);
        }

        #endregion

        #region DeleteNews

        [Fact]
        public async Task DeleteNews_Should_Return_Success_When_News_Exists()
        {
            // Arrange
            var newsId = "news123";
            var existingNews = new News
            {
                Id = newsId,
                Title = "Test News",
                Status = "Active"
            };

            var newsResponse = new NewsResponse
            {
                Id = newsId,
                Title = "Test News",
                Status = NewsStatus.Deleted.ToString()
            };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync(existingNews);
            _newsRepositoryMock.Setup(r => r.Update(It.IsAny<News>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<NewsResponse>(It.IsAny<News>())).Returns(newsResponse);

            // Act
            var result = await _newsService.DeleteNews(newsId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete news successfully", result.Message);
            Assert.Equal(newsResponse, result.Data);
            Assert.Equal(NewsStatus.Deleted.ToString(), existingNews.Status);
        }

        [Fact]
        public async Task DeleteNews_Should_Return_Failure_When_Id_Is_Empty()
        {
            // Arrange
            string newsId = string.Empty;

            // Act
            var result = await _newsService.DeleteNews(newsId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("News ID cannot be empty", result.Message);
        }

        [Fact]
        public async Task DeleteNews_Should_Return_Failure_When_News_Not_Found()
        {
            // Arrange
            var newsId = "nonexistent";
            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync((News)null);

            // Act
            var result = await _newsService.DeleteNews(newsId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("News not found", result.Message);
        }

        [Fact]
        public async Task DeleteNews_Should_Return_Failure_When_News_Already_Deleted()
        {
            // Arrange
            var newsId = "news123";
            var existingNews = new News
            {
                Id = newsId,
                Title = "Test News",
                Status = NewsStatus.Deleted.ToString()
            };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync(existingNews);

            // Act
            var result = await _newsService.DeleteNews(newsId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("News is already deleted", result.Message);
        }

        [Fact]
        public async Task DeleteNews_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            var newsId = "news123";
            var existingNews = new News
            {
                Id = newsId,
                Title = "Test News",
                Status = "Active"
            };

            _newsRepositoryMock.Setup(r => r.GetNewsById(newsId)).ReturnsAsync(existingNews);
            _newsRepositoryMock.Setup(r => r.Update(It.IsAny<News>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _newsService.DeleteNews(newsId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to delete news", result.Message);
        }

        #endregion

        #region GetNewsByUserId

        [Fact]
        public async Task GetNewsByUserId_Should_Return_Success_When_UserId_Is_Valid()
        {
            // Arrange
            var userId = "user123";
            var newsList = new List<News>
            {
                new News { Id = "news1", Title = "News 1", UserId = userId },
                new News { Id = "news2", Title = "News 2", UserId = userId }
            };

            var newsResponseList = new List<NewsResponse>
            {
                new NewsResponse { Id = "news1", Title = "News 1", UserId = userId },
                new NewsResponse { Id = "news2", Title = "News 2", UserId = userId }
            };

            _newsRepositoryMock.Setup(r => r.GetNewsByUserId(userId)).ReturnsAsync(newsList);
            _mapperMock.Setup(m => m.Map<List<NewsResponse>>(newsList)).Returns(newsResponseList);

            // Act
            var result = await _newsService.GetNewsByUserId(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Get news by user successfully", result.Message);
            Assert.Equal(newsResponseList, result.Data);
        }

        [Fact]
        public async Task GetNewsByUserId_Should_Return_Failure_When_UserId_Is_Empty()
        {
            // Arrange
            string userId = string.Empty;

            // Act
            var result = await _newsService.GetNewsByUserId(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User ID cannot be empty", result.Message);
        }

        [Fact]
        public async Task GetNewsByUserId_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            var userId = "user123";
            _newsRepositoryMock.Setup(r => r.GetNewsByUserId(userId)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _newsService.GetNewsByUserId(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to get news", result.Message);
        }

        #endregion
    }
} 