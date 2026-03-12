using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Person;
using Eigakan.Domain.Response.Person;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class PersonServiceTest
    {
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        
        private readonly PersonService _personService;

        private readonly Mock<IConfiguration> _configurationMock;



        private readonly Mock<Logger> _loggerMock;
        private readonly Mock<IOptions<DiscordWebhookUrls>> _optionsMock;
        private readonly Mock<Webhook> _webhookMock;
        private readonly Logger _logger;

        public PersonServiceTest()
        {
            _optionsMock = new Mock<IOptions<DiscordWebhookUrls>>();

            // Setup mock webhook URL
            _optionsMock.Setup(o => o.Value).Returns(new DiscordWebhookUrls
            {
                AdminUpdates = ""
            });

            // Mock Webhook with the provided options
            _webhookMock = new Mock<Webhook>(_optionsMock.Object);

            // Initialize logger
            _logger = new Logger(_webhookMock.Object);

            _personRepositoryMock = new Mock<IPersonRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<Logger>();
            _personService = new PersonService(_personRepositoryMock.Object, _mapperMock.Object, _logger);
        }
        //[Fact]
        //public async Task CreatePerson_ShouldHandleException()
        //{
        //    // Arrange
        //    var request = new PersonCreateRequest { Name = "Crash" };
        //    _personRepositoryMock.Setup(repo => repo.Insert(It.IsAny<Person>()))
        //        .ThrowsAsync(new Exception("Insert failed"));

        //    // Act
        //    var result = await _personService.CreatePerson(request);

        //    // Assert
        //    Assert.False(result.Success);
        //    Assert.Equal("Insert failed", result.Message);
        //}
        [Fact]
        public async Task CreatePerson_ShouldReturnSuccess()
        {
            var request = new PersonCreateRequest
            {
                Name = "John Doe",
                Birthday =  "13/7/2003",
                Gender = true,
                Job = "Actor",
                Description = "Famous actor",
                Picture = "img.png"
            };

            var result = await _personService.CreatePerson(request);

            Assert.True(result.Success);
            Assert.Equal("success", result.Message);
            Assert.Equal(request.Name, result.Data.Name);
        }

        [Fact]
        public async Task GetList_ShouldReturnMappedList()
        {
            var personList = new List<Person> { new Person { Name = "Test Person" } };
            var responseList = new List<PersonListResponse> { new PersonListResponse { Name = "Test Person" } };

            _personRepositoryMock.Setup(r => r.GetList(1, 10, null, null)).ReturnsAsync(personList);
            _mapperMock.Setup(m => m.Map<List<PersonListResponse>>(personList)).Returns(responseList);

            var result = await _personService.GetList(1, 10, null, null);

            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal("Test Person", result.Data[0].Name);
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnPersonWithActiveMovies()
        {
            var personId = "123";
            var person = new Person
            {
                Id = personId,
                Name = "Person A",
                MoviePersons = new List<MoviePerson>
            {
                new MoviePerson { Movie = new Movie { Id = "1", Status = "ACTIVE", Title = "Movie 1", OriginName = "Origin 1", Media = new List<Media> { new Media { Type = "POSTER", Url = "url1" } } } }
            }
            };

            _personRepositoryMock.Setup(r => r.GetPersById(personId)).ReturnsAsync(person);

            var result = await _personService.GetPersonById(personId);

            Assert.True(result.Success);
            Assert.Equal(personId, result.Data.Id);
            Assert.Single(result.Data.movieList);
            Assert.Equal("url1", result.Data.movieList[0].Medias);
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnNotFound()
        {
            // Arrange
            _personRepositoryMock.Setup(repo => repo.GetPersById("404"))
                .ReturnsAsync((Person)null);

            // Act
            var result = await _personService.GetPersonById("404");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Not found", result.Message);
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnUpdatedPerson()
        {
            var id = "abc123";
            var request = new PersonCreateRequest
            {
                Name = "Updated",
                Birthday = "13/7/2003",
                Description = "Updated desc",
                Gender = true,
                Job = "Director",
                Picture = "updated.png"
            };
            var existingPerson = new Person { Id = id };

            _personRepositoryMock.Setup(r => r.GetPersById(id)).ReturnsAsync(existingPerson);

            var result = await _personService.UpdatePerson(id, request);

            Assert.True(result.Success);
            Assert.Equal("Updated", result.Data.Name);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnSuccessIfDeleted()
        {
            var id = "delete123";
            _personRepositoryMock.Setup(r => r.GetPersById(id)).ReturnsAsync(new Person { Id = id });
            _personRepositoryMock.Setup(r => r.DeletePersonAsync(id)).ReturnsAsync(true);

            var result = await _personService.DeletePerson(id);

            Assert.True(result.Success);
            Assert.Equal("Delete success", result.Message);
        }
        [Fact]
        public async Task DeletePerson_ShouldReturnNotFound()
        {
            // Arrange
            _personRepositoryMock.Setup(repo => repo.GetPersById("missing"))
                .ReturnsAsync((Person)null);

            // Act
            var result = await _personService.DeletePerson("missing");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Not found", result.Message);
        }
    }
}
