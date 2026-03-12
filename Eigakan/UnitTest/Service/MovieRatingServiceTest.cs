using Xunit;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Request.MovieRating;
using Eigakan.Application.Helper.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Runtime.ConstrainedExecution;



namespace UnitTest.Service
{
    public class MovieRatingServiceTest
    {



        private readonly Mock<IMovieRatingRepository> _movieRatingRepoMock;
        private readonly Mock<IMoviesRepository> _moviesRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        private readonly MovieRatingService _service;


        private readonly Mock<IConfiguration> _configurationMock;



        private readonly Mock<Logger> _loggerMock;
        private readonly Mock<IOptions<DiscordWebhookUrls>> _optionsMock;
        private readonly Mock<Webhook> _webhookMock;
        private readonly Logger _logger;
        public MovieRatingServiceTest()
        {
            _movieRatingRepoMock = new Mock<IMovieRatingRepository>();
            _moviesRepositoryMock = new Mock<IMoviesRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _loggerMock = new Mock<Logger>();
            _configurationMock = new Mock<IConfiguration>();

            _httpContextAccessorMock.Setup(h => h.HttpContext)
            .Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(MySetting.CLAIM_USERID, "test-user-id")
                }))
            });
            _optionsMock = new Mock<IOptions<DiscordWebhookUrls>>();

            // Setup mock webhook URL
            _optionsMock.Setup(o => o.Value).Returns(new DiscordWebhookUrls
            {
                AdminUpdates = "https://discord.com/api/webhooks/1326070733650137139/qG7-RocMHVj0CFlw3vAr2YGE2Ou7QJfOA9jwzL7TgZj0JJTgYkSUBFg_yfWMWPYhjLs-"
            });

            // Mock Webhook with the provided options
            _webhookMock = new Mock<Webhook>(_optionsMock.Object);

            // Initialize logger
            _logger = new Logger(_webhookMock.Object);


            _service = new MovieRatingService(
                _movieRatingRepoMock.Object,
                _moviesRepositoryMock.Object,
                _httpContextAccessorMock.Object,
               _logger);
        }

        private void SetupHttpContext(string userId, string role = "User")
        {


            var claims = new List<Claim>
        {
            new Claim(MySetting.CLAIM_USERID, userId),
            new Claim(ClaimTypes.Role, role)
        };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async Task Rating_ShouldReturnSuccess_WhenNewRatingCreated()
        {
            // Arrange
            var movieId = "movie-1";
            var userId = "user-1";
            SetupHttpContext(userId);
            var request = new MovieRatingCreateRequest { MovieId = movieId, Stars = 5 };

            _moviesRepositoryMock.Setup(m => m.GetMovieById(movieId)).ReturnsAsync(new Movie { Id = movieId });
            _movieRatingRepoMock.Setup(r => r.GetMovieRatingByUserId(userId, movieId)).ReturnsAsync((MovieRating)null);
            _movieRatingRepoMock.Setup(r => r.Insert(It.IsAny<MovieRating>())).Returns(Task.CompletedTask);
            _movieRatingRepoMock.Setup(r => r.GetMovieRatingById(It.IsAny<string>()))
                .ReturnsAsync(new MovieRating { Id = Guid.NewGuid().ToString(), Stars = 5 });

            // Act
            var result = await _service.Rating(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Create success!", result.Message);
            Assert.Equal(5, result.Data.Stars);
        }
        [Fact]
        public async Task Update_ShouldReturnUnauthorized_WhenUserIsNotOwner()
        {
            // Arrange
            var ratingId = "rating-123";
            var actualUserId = "user-456";
            var fakeUserId = "user-789";

            var claims = new List<Claim>
    {
        new Claim(MySetting.CLAIM_USERID, fakeUserId),
        new Claim(ClaimTypes.Role, "User")
    };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var rating = new MovieRating
            {
                Id = ratingId,
                UserId = actualUserId,
                MovieId = "movie-1",
                Stars = 3
            };

            _movieRatingRepoMock.Setup(r => r.GetMovieRatingById(ratingId)).ReturnsAsync(rating);

            var request = new MovieRatingUpdateRequest
            {
                Stars = 5
            };

            // Act
            var result = await _service.Update(ratingId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("YOU DON'T HAVE AUTHORIZATION", result.Message);
        }
        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenUserIsOwnerAndRatingExists()
        {
            // Arrange
            var ratingId = "rating-123";
            var userId = "user-456";
            var newStars = 4;

            // Setup HttpContext with claim
            var claims = new List<Claim>
    {
        new Claim(MySetting.CLAIM_USERID, userId),
        new Claim(ClaimTypes.Role, "User")
    };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var existingRating = new MovieRating
            {
                Id = ratingId,
                MovieId = "movie-1",
                UserId = userId,
                Stars = 2
            };

            _movieRatingRepoMock.Setup(r => r.GetMovieRatingById(ratingId)).ReturnsAsync(existingRating);
            _movieRatingRepoMock.Setup(r => r.Update(It.IsAny<MovieRating>())).Returns(Task.CompletedTask);
            _movieRatingRepoMock.Setup(r => r.GetMovieRatingById(ratingId)).ReturnsAsync(
                new MovieRating
                {
                    Id = ratingId,
                    UserId = userId,
                    MovieId = "movie-1",
                    Stars = newStars
                });

            var request = new MovieRatingUpdateRequest
            {
                Stars = newStars
            };

            // Act
            var result = await _service.Update(ratingId, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Update success", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(newStars, result.Data.Stars);
        }

        [Fact]
        public async Task GetMovieRatingByLogin_ShouldReturnRating_WhenExists()
        {
            // Arrange
            var movieId = "movie-1";
            var userId = "user-1";
            SetupHttpContext(userId);
            var rating = new MovieRating { MovieId = movieId, UserId = userId, Stars = 5 };

            _movieRatingRepoMock.Setup(r => r.GetMovieRatingByLogin(userId, movieId)).ReturnsAsync(rating);

            // Act
            var result = await _service.GetMovieRatingByLogin(movieId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(5, result.Data.Stars);
        }
    }
}
