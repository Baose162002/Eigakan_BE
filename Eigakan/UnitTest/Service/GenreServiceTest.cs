using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface.IRepository;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eigakan.Application.Interface;
using AutoMapper;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Genre;
using Eigakan.Domain.Response.Genre;
using Xunit;

namespace UnitTest.Service
{
    public class GenreServiceTest
    {
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GenreService _genreService;
        public GenreServiceTest()
        {
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _mapperMock = new Mock<IMapper>();
            _genreService = new GenreService(_genreRepositoryMock.Object, _mapperMock.Object);
        }

        public async Task CreateGenre_ShouldReturnSuccess_WhenGenreIsNew()
        {
            // Arrange
            var request = new CreateGenreRequest
            {
                Name = "Action",
                Description = "Exciting and fast-paced"
            };

            _genreRepositoryMock.Setup(r => r.CheckName("Action")).ReturnsAsync(0);
            _genreRepositoryMock.Setup(r => r.Insert(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            var result = await _genreService.CreateGenre(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("success", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal("Action", result.Data.Name);
        }

        [Fact]
        public async Task CreateGenre_ShouldReturnError_WhenGenreAlreadyExists()
        {
            // Arrange
            var request = new CreateGenreRequest { Name = "Drama",Description="aaaaaaaaaa" };
            _genreRepositoryMock.Setup(r => r.CheckName("Drama")).ReturnsAsync(1);

            // Act
            var result = await _genreService.CreateGenre(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Already have this genre!!!", result.Message);
        }
        [Fact]
        public async Task CreateGenre_ShouldCreateSuccessfully_WhenNameIsUnique()
        {
            // Arrange
            var genreRequest = new CreateGenreRequest
            {
                Name = "Fantasy",
                Description = "Magical world"
            };

            _genreRepositoryMock.Setup(r => r.CheckName("Fantasy")).ReturnsAsync(0);
            _genreRepositoryMock.Setup(r => r.Insert(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            var result = await _genreService.CreateGenre(genreRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("success", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal("Fantasy", result.Data.Name);
            Assert.Equal("Magical world", result.Data.Description);
            Assert.False(string.IsNullOrEmpty(result.Data.Id));
        }


        [Fact]
        public async Task CreateGenre_ShouldHandleException()
        {
            // Arrange
            var request = new CreateGenreRequest { Name = "Mystery", Description = "Dark vibes" };

            _genreRepositoryMock.Setup(r => r.CheckName(It.IsAny<string>())).ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await _genreService.CreateGenre(request);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("DB failure", result.Message);
        }

        [Fact]
        public async Task GetList_ShouldReturnMappedGenres()
        {
            // Arrange
            var genres = new List<Genre>
        {
            new Genre { Id = "1", Name = "Comedy", Description = "Funny stuff" }
        };
            var mapped = new List<GenreListNameResponse>
        {
            new GenreListNameResponse { Id = "1", Name = "Comedy" }
        };

            _genreRepositoryMock.Setup(r => r.GetList()).ReturnsAsync(genres);
            _mapperMock.Setup(m => m.Map<List<GenreListNameResponse>>(genres)).Returns(mapped);

            // Act
            var result = await _genreService.GetList();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(mapped, result.Data);
        }

        [Fact]
        public async Task DeleteGenre_ShouldReturnSuccess_WhenGenreIsDeleted()
        {
            // Arrange
            var genre = new Genre { Id = "1", Name = "Sci-Fi" };
            _genreRepositoryMock.Setup(r => r.GetGenreById("1")).ReturnsAsync(genre);
            _genreRepositoryMock.Setup(r => r.DeleteGenreAsync("1")).ReturnsAsync(false);

            // Act
            var result = await _genreService.DeleteGenre("1");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete  success", result.Message);
        }

        [Fact]
        public async Task UpdateGenre_ShouldReturnUpdatedGenre_WhenFound()
        {
            // Arrange
            var genre = new Genre { Id = "1", Name = "Old Name", Description = "Old Description" };
            var updateRequest = new GenreUpdateRequest { Name = "New Name", Description = "New Desc" };

            _genreRepositoryMock.Setup(r => r.GetGenreById("1")).ReturnsAsync(genre);
            _genreRepositoryMock.Setup(r => r.Update(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            var result = await _genreService.UpdateGenre("1", updateRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("New Name", result.Data.Name);
        }

        [Fact]
        public async Task UpdateGenre_ShouldHandleException()
        {
            // Arrange
            var genre = new Genre { Id = "1", Name = "Old", Description = "Old Desc" };
            _genreRepositoryMock.Setup(r => r.GetGenreById("1")).ReturnsAsync(genre);
            _genreRepositoryMock.Setup(r => r.Update(It.IsAny<Genre>())).ThrowsAsync(new Exception("Update failed"));

            var request = new GenreUpdateRequest { Name = "New", Description = "New" };

            // Act
            var result = await _genreService.UpdateGenre("1", request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Update failed", result.Message);
        }


        [Fact]
        public async Task UpdateGenre_ShouldReturnNotFound_WhenGenreDoesNotExist()
        {
            // Arrange
            _genreRepositoryMock.Setup(r => r.GetGenreById("nope")).ReturnsAsync((Genre)null);

            var updateRequest = new GenreUpdateRequest { Name = "New", Description = "New Desc" };

            // Act
            var result = await _genreService.UpdateGenre("nope", updateRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Not found", result.Message);
        }


        [Fact]
        public async Task GetGenreById_ShouldReturnMappedGenreWithMovies()
        {
            // Arrange
            var genreId = "1";
            var genre = new Genre
            {
                Id = genreId,
                Name = "Thriller",
                Description = "Suspense genre",
                MovieGenres = new List<MovieGenre>
        {
            new MovieGenre
            {
                Movie = new Movie
                {
                    Id = "M1",
                    Title = "Movie One",
                    OriginName = "Origin One",
                    Status = "ACTIVE",
                    Media = new List<Media>
                    {
                        new Media { Type = "POSTER", Url = "poster-url" }
                    }
                }
            },
            new MovieGenre
            {
                Movie = new Movie
                {
                    Id = "M2",
                    Title = "Movie Two",
                    OriginName = "Origin Two",
                    Status = "INACTIVE",
                    Media = new List<Media>
                    {
                        new Media { Type = "POSTER", Url = "poster-url-2" }
                    }
                }
            }
        }
            };

            _genreRepositoryMock.Setup(r => r.GetGenreById(genreId)).ReturnsAsync(genre);

            // Act
            var result = await _genreService.GetGenreById(genreId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(genreId, result.Data.Id);
            Assert.Single(result.Data.movieList); // Only ACTIVE movie should be included
            Assert.Equal("Movie One", result.Data.movieList[0].Title);
            Assert.Equal("poster-url", result.Data.movieList[0].Medias);
        }


        [Fact]
        public async Task GetList_ShouldHandleException()
        {
            // Arrange
            _genreRepositoryMock.Setup(r => r.GetList()).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _genreService.GetList();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong", result.Message);
        }

        [Fact]
        public async Task GetGenreById_ShouldHandleException()
        {
            // Arrange
            _genreRepositoryMock.Setup(r => r.GetGenreById("123")).ThrowsAsync(new Exception("Boom"));

            // Act
            var result = await _genreService.GetGenreById("123");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Boom", result.Message);
        }

       
        [Fact]
        public async Task DeleteGenre_ShouldHandleException()
        {
            // Arrange
            var genre = new Genre { Id = "123", Name = "Horror" };
            _genreRepositoryMock.Setup(r => r.GetGenreById("123")).ReturnsAsync(genre);
            _genreRepositoryMock.Setup(r => r.DeleteGenreAsync("123")).ThrowsAsync(new Exception("Cannot delete"));

            // Act
            var result = await _genreService.DeleteGenre("123");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot delete", result.Message);
        }

        [Fact]
        public async Task DeleteGenre_ShouldReturnFail_WhenGenreNotFound()
        {
            // Arrange
            _genreRepositoryMock.Setup(r => r.GetGenreById("404")).ReturnsAsync((Genre)null);

            // Act
            var result = await _genreService.DeleteGenre("404");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Not found", result.Message);
        }

    }
}
