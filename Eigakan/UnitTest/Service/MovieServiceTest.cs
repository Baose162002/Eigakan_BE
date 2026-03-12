using Amazon;
using Amazon.S3;
using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Media;
using Eigakan.Domain.Request.Movie;
using Eigakan.Domain.Response.Movie;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class MovieServiceTests
    {
        private readonly Mock<IMoviesRepository> _moviesRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<IMoviePersonRepository> _moviePersonRepositoryMock;
        private readonly Mock<IMovieGenreRepository> _movieGenreRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IMovieRatingRepository> _movieRatingRepositoryMock;
        private readonly Mock<IMediaRepository> _mediaRepositoryMock ;
         
        private readonly Mock<IConfiguration> _configurationMock;
       
      

        private readonly Mock<Logger> _loggerMock;
        private readonly Mock<IOptions<DiscordWebhookUrls>> _optionsMock;
        private readonly Mock<Webhook> _webhookMock;
        private readonly Logger _logger;

        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _loggerMock = new Mock<Logger>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _mapperMock = new Mock<IMapper>();
            _moviesRepositoryMock= new Mock<IMoviesRepository> ();
            _personRepositoryMock= new Mock<IPersonRepository> ();
            _movieGenreRepositoryMock= new Mock<IMovieGenreRepository> ();
            _moviePersonRepositoryMock= new Mock<IMoviePersonRepository> ();
            _movieRatingRepositoryMock= new Mock<IMovieRatingRepository> ();
            _mediaRepositoryMock= new Mock<IMediaRepository> ();
            _configurationMock= new Mock<IConfiguration> ();            
            _httpContextAccessorMock=new Mock<IHttpContextAccessor> ();
            _commentRepositoryMock= new Mock<ICommentRepository> ();

            
            // Set up AWS config values
            _configurationMock.Setup(c => c["AWS:BucketName"]).Returns("test-bucket");
            _configurationMock.Setup(c => c["AWS:AccessKey"]).Returns("fake-access-key");
            _configurationMock.Setup(c => c["AWS:SecretKey"]).Returns("fake-secret-key");
            _configurationMock.Setup(c => c["AWS:Region"]).Returns("ap-southeast-1");

            _httpContextAccessorMock.Setup(h => h.HttpContext)
    .Returns(new DefaultHttpContext
    {
        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(MySetting.CLAIM_USERID, "test-user-id")
        }))
    });

            _optionsMock = new Mock<IOptions<DiscordWebhookUrls>>();

          
            _optionsMock.Setup(o => o.Value).Returns(new DiscordWebhookUrls
            {
                AdminUpdates = "https://discord.com/api/webhooks/1326070733650137139/qG7-RocMHVj0CFlw3vAr2YGE2Ou7QJfOA9jwzL7TgZj0JJTgYkSUBFg_yfWMWPYhjLs-"
            });

      
            _webhookMock = new Mock<Webhook>(_optionsMock.Object);

            _logger = new Logger(_webhookMock.Object);

            _movieService = new MovieService(
               _logger,
                _moviesRepositoryMock.Object,
                _genreRepositoryMock.Object,
                _personRepositoryMock.Object,
                _mapperMock.Object,
                _movieGenreRepositoryMock.Object,
                _moviePersonRepositoryMock.Object,
               _httpContextAccessorMock.Object,
                _commentRepositoryMock.Object,
                _movieRatingRepositoryMock.Object,
                _mediaRepositoryMock.Object,
               _configurationMock.Object
            );
        }

        [Fact]
        public async Task CreateMovie_ShouldReturnSuccess_WhenValidRequest()
        {
            var request = new CreateMovieRequest
            {
                Title = "Test Movie",
                Description = "Description",
                Director = "Director",
                Duration = 120,
                Nation = "VN",
                OriginName = "VERY HARD TO TEST",
                IsContract = true,
                ReleaseYear = "2025",
                Script = "Script",
                FileUrl = "https://example.com/movie.mp4",
                Genres = new List<string> { "genre1" },
                Persons = new List<string> { "person1" }
            };

            _genreRepositoryMock.Setup(r => r.GetListGenreById(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<string> { "genre1" });

            _personRepositoryMock.Setup(r => r.GetListPersonById(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<string> { "person1" });

            _moviesRepositoryMock.Setup(r => r.Insert(It.IsAny<Movie>()))
                .Returns(Task.CompletedTask);

            _moviesRepositoryMock.Setup(r => r.GetMovieById(It.IsAny<string>()))
                .ReturnsAsync(new Movie
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Test Movie",
                    MovieGenres = new List<MovieGenre>(),
                    MoviePersons = new List<MoviePerson>()
                });

            _mapperMock.Setup(m => m.Map<MovieGetListResponse>(It.IsAny<Movie>()))
                .Returns(new MovieGetListResponse { Title = "Test Movie" });

            _movieRatingRepositoryMock.Setup(r => r.GetAverageRating(It.IsAny<string>()))
                .ReturnsAsync(4.5);

            // Act
            var result = await _movieService.CreateMovie(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            //Assert.Equal("Test Movie", result.Data.Title);
        }
       
        [Fact]
        public async Task RejectedMovie_ShouldReject_WhenInReviewingStatus()
        {
            var request = new RejectedMovieRequest
            {
                Id = "movie-4",
                ReasonForRejection = "Not suitable"
            };
            var movie = new Movie { Id = request.Id, Status = "WAITING_FOR_REVIEWING" };

            _moviesRepositoryMock.Setup(x => x.GetMovieById(request.Id)).ReturnsAsync(movie);
            _moviesRepositoryMock.Setup(x => x.Update(It.IsAny<Movie>())).Returns(Task.CompletedTask);

            var result = await _movieService.RejectedMovie(request);

            Assert.True(result.Success);
            Assert.Equal(MovieStatusEnum.REJECTED.ToString(), movie.Status);
            Assert.Equal(request.ReasonForRejection, movie.ReasonForRejection);
        }

        [Fact]
        public async Task GetByMovieIdClear_ShouldReturnMovie_WhenExists()
        {
            // Arrange
            var movieId = "movie-1";

            var movie = new Movie
            {
                Id = movieId,
                MovieGenres = new List<MovieGenre>
        {
            new MovieGenre
            {
                Genre = new Genre { Name = "Action" }
            }
        },
                MoviePersons = new List<MoviePerson>
        {
            new MoviePerson
            {
                Person = new Person { Name = "John Doe" }
            }
        }
            };

            var movieDto = new MovieGetById
            {
                Id = movieId
                // Other properties can be set if needed
            };

            _moviesRepositoryMock.Setup(r => r.GetMovieById(movieId)).ReturnsAsync(movie);
            _mapperMock.Setup(m => m.Map<MovieGetById>(It.IsAny<Movie>())).Returns(movieDto);
            _commentRepositoryMock.Setup(r => r.GetListMovieId(movieId)).ReturnsAsync(new List<Comment>());
            _movieRatingRepositoryMock.Setup(r => r.GetAverageRating(movieId)).ReturnsAsync(4.5);

            // Act
            var result = await _movieService.GetByMovieIdClear(movieId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(movieId, result.Data.Id);
            Assert.Equal("Action", result.Data.GenreNames);
            Assert.Equal(4.5, result.Data.UserRating);
            Assert.NotNull(result.Data.Person);
            Assert.Single(result.Data.Person);
            Assert.Equal("John Doe", result.Data.Person.First().Name);
        }
        [Fact]
        public async Task UpdateMovie_ShouldUpdate_WhenUserIsAdminAndDataIsValid()
        {
            // Arrange
            var movieId = "movie-1";
            var userId = "user-123";

            var existingMovie = new Movie
            {
                Id = movieId,
                UserId = userId,
                MovieGenres = new List<MovieGenre>
        {
            new MovieGenre { GenreId = "genre-1" }
        },
                MoviePersons = new List<MoviePerson>
        {
            new MoviePerson { PersonId = "person-1" }
        },
                Media = new List<Media>
        {
            new Media { Id = "media-1", MovieId = movieId }
        }
            };

            var movieRequest = new UpdateMovieRequest
            {
                Title = "Updated Title",
                Genres = new List<string> { "genre-2" },
                Persons = new List<string> { "person-2" },
                Medias = new List<MediaMovieCreateRequest>
        {
            new MediaMovieCreateRequest
            {
                Name = "Poster",
                Type = "Image",
                Url = "https://example.com/poster.jpg"
            }
        }
            };

            _moviesRepositoryMock.Setup(r => r.GetMovieById(movieId)).ReturnsAsync(existingMovie);
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Role, "ADMIN"),
            new Claim(MySetting.CLAIM_USERID, userId)
        }))
            });

            _genreRepositoryMock.Setup(r => r.GetListGenreById(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<string> { "genre-2" });

            _personRepositoryMock.Setup(r => r.GetListPersonById(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<string> { "person-2" });

            _movieGenreRepositoryMock.Setup(r => r.Insert(It.IsAny<MovieGenre>()))
                .Returns(Task.CompletedTask);

            _movieGenreRepositoryMock.Setup(r => r.DeleteRange(It.IsAny<List<MovieGenre>>()))
                .Returns(Task.CompletedTask);

            _moviePersonRepositoryMock.Setup(r => r.Insert(It.IsAny<MoviePerson>()))
                .Returns(Task.CompletedTask);

            _moviePersonRepositoryMock.Setup(r => r.DeleteRange(It.IsAny<List<MoviePerson>>()))
                .Returns(Task.CompletedTask);

            _mediaRepositoryMock.Setup(r => r.DeleteRange(It.IsAny<List<Media>>()))
                .Returns(Task.CompletedTask);

            _mediaRepositoryMock.Setup(r => r.Insert(It.IsAny<Media>()))
                .Returns(Task.CompletedTask);

            _moviesRepositoryMock.Setup(r => r.Update(It.IsAny<Movie>()))
                .Returns(Task.CompletedTask);

            _moviesRepositoryMock.Setup(x => x.GetMovieById(movieId)).ReturnsAsync(existingMovie);

           
            _mapperMock.Setup(m => m.Map<MovieGetListResponse>(It.IsAny<Movie>())).Returns(new MovieGetListResponse
            {
                Id = movieId,
                Title = movieRequest.Title
            });

            _commentRepositoryMock.Setup(x => x.GetListMovieId(movieId)).ReturnsAsync(new List<Comment>());
            _movieRatingRepositoryMock.Setup(x => x.GetAverageRating(movieId)).ReturnsAsync(4.5);

          
            var result = await _movieService.UpdateMovie(movieId, movieRequest);


            
           

            
            Assert.True(result.Success);
            Assert.Equal(movieId, result.Data.Id);
            _moviesRepositoryMock.Verify(r => r.Update(It.Is<Movie>(m => m.Title == "Updated Title")), Times.Once);
        }

    }



}
