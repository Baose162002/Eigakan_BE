using AutoMapper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.SubscriptionPackageRequest;
using Eigakan.Domain.Response.SubscriptionPackageResponse;
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
    public class SubscriptionPackageServiceTest
    {
        private readonly Mock<ISubscriptionPackageRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<SubscriptionPackageService>> _mockLogger;
        private readonly SubscriptionPackageService _service;

        public SubscriptionPackageServiceTest()
        {
            _mockRepo = new Mock<ISubscriptionPackageRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<SubscriptionPackageService>>();
            _service = new SubscriptionPackageService(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }
        #region GetAll
        [Fact]
        public async Task GetAllSubscriptionPackageAsync_ShouldReturnSuccess()
        {
            // Arrange
            var packages = new List<SubscriptionPackage> { new SubscriptionPackage { Id = "1", PackageName = "Basic" } };
            var mapped = new List<SubscriptionPackageGetAllResponse> { new SubscriptionPackageGetAllResponse { PackageName = "Basic" } };

            _mockRepo.Setup(x => x.GetAllSubscriptionPackage(1, 10)).ReturnsAsync(packages);
            _mockRepo.Setup(x => x.CountAllSubscriptionPackageAsync()).ReturnsAsync(1);
            _mockMapper.Setup(x => x.Map<List<SubscriptionPackageGetAllResponse>>(packages)).Returns(mapped);

            // Act
            var result = await _service.GetAllSubscriptionPackageAsync(1, 10);

            // Assert
            Assert.True(result.Success);
            Assert.Single(result.Data.SubscriptionPackages);
            Assert.Equal("Basic", result.Data.SubscriptionPackages[0].PackageName);
        }
        [Fact]
        public async Task GetAllSubscriptionPackageAsync_ShouldReturnFailure_WhenInvalidPagination()
        {
            var result = await _service.GetAllSubscriptionPackageAsync(0, -5);

            Assert.False(result.Success);
            Assert.Equal("Invalid page or pageSize values.", result.Message);
        }
        #endregion

        #region GetById
        [Fact]
        public async Task GetSubscriptionPackageById_ShouldReturnSuccess_WhenPackageExists()
        {
            var id = "123";
            var entity = new SubscriptionPackage { Id = id, PackageName = "Premium" };
            var dto = new SubscriptionPackageGetAllResponse { PackageName = "Premium" };

            _mockRepo.Setup(x => x.GetSubscriptionPackageById(id)).ReturnsAsync(entity);
            _mockMapper.Setup(x => x.Map<SubscriptionPackageGetAllResponse>(entity)).Returns(dto);

            var result = await _service.GetSubscriptionPackageById(id);

            Assert.True(result.Success);
            Assert.Equal("Premium", result.Data.PackageName);
        }
        [Fact]
        public async Task GetSubscriptionPackageById_ShouldReturnFailure_WhenIdIsNull()
        {
            var result = await _service.GetSubscriptionPackageById(null);

            Assert.False(result.Success);
            Assert.Equal("Id must not be null or empty.", result.Message);
        }
        [Fact]
        public async Task GetSubscriptionPackageById_ShouldReturnFailure_WhenNotFound()
        {
            _mockRepo.Setup(x => x.GetSubscriptionPackageById("invalid")).ReturnsAsync((SubscriptionPackage)null);

            var result = await _service.GetSubscriptionPackageById("invalid");

            Assert.False(result.Success);
            Assert.Equal("SubscriptionPackage with the specified Id does not exist.", result.Message);
        }

        #endregion
        #region Create
        [Fact]
        public async Task CreateSubscriptionPackageAsync_ShouldReturnSuccess_WhenValidRequest()
        {
            var request = new SubscriptionPackageCreateRequest
            {
                PackageName = "Standard",
                Price = 100,
                Duration = 30
            };

            _mockRepo.Setup(x => x.Insert(It.IsAny<SubscriptionPackage>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(x => x.Map<SubscriptionPackageGetAllResponse>(It.IsAny<SubscriptionPackage>()))
                       .Returns(new SubscriptionPackageGetAllResponse { PackageName = "Standard" });

            var result = await _service.CreateSubscriptionPackageAsync(request);

            Assert.True(result.Success);
            Assert.Equal("Standard", result.Data.PackageName);
        }
        [Fact]
        public async Task CreateSubscriptionPackageAsync_ShouldReturnFailure_WhenNameIsEmpty()
        {
            var request = new SubscriptionPackageCreateRequest
            {
                PackageName = "",
                Price = 100,
                Duration = 30
            };

            var result = await _service.CreateSubscriptionPackageAsync(request);

            Assert.False(result.Success);
            Assert.Equal("PackageName cannot be empty.", result.Message);
        }
        [Fact]
        public async Task CreateSubscriptionPackageAsync_ShouldReturnFailure_WhenPriceIsInvalid()
        {
            var request = new SubscriptionPackageCreateRequest
            {
                PackageName = "Test",
                Price = 0,
                Duration = 30
            };

            var result = await _service.CreateSubscriptionPackageAsync(request);

            Assert.False(result.Success);
            Assert.Equal("Price must be greater than 0.", result.Message);
        }
        [Fact]
        public async Task CreateSubscriptionPackageAsync_ShouldLogAndReturnFailure_WhenExceptionThrown()
        {
            var request = new SubscriptionPackageCreateRequest
            {
                PackageName = "Premium",
                Price = 100,
                Duration = 30
            };

            _mockRepo.Setup(x => x.Insert(It.IsAny<SubscriptionPackage>())).ThrowsAsync(new Exception("Database error"));

            var result = await _service.CreateSubscriptionPackageAsync(request);

            Assert.False(result.Success);
            Assert.Contains("Failed to generate contract", result.Message);
        }



        #endregion
        #region Update
        [Fact]
        public async Task UpdateSubscriptionPackageAsync_ShouldReturnSuccess_WhenValidUpdate()
        {
            var id = "123";
            var request = new SubscriptionPackageUpdateRequest
            {
                PackageName = "Updated",
                Price = 200,
                Duration = 60
            };
            var entity = new SubscriptionPackage { Id = id };

            _mockRepo.Setup(x => x.GetSubscriptionPackageById(id)).ReturnsAsync(entity);
            _mockRepo.Setup(x => x.Update(It.IsAny<SubscriptionPackage>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(x => x.Map<SubscriptionPackageGetAllResponse>(It.IsAny<SubscriptionPackage>()))
                       .Returns(new SubscriptionPackageGetAllResponse { PackageName = "Updated" });

            var result = await _service.UpdateSubscriptionPackageAsync(id, request);

            Assert.True(result.Success);
            Assert.Equal("Updated", result.Data.PackageName);
        }
        [Fact]
        public async Task UpdateSubscriptionPackageAsync_ShouldReturnFailure_WhenNotFound()
        {
            _mockRepo.Setup(x => x.GetSubscriptionPackageById("notfound")).ReturnsAsync((SubscriptionPackage)null);

            var request = new SubscriptionPackageUpdateRequest
            {
                PackageName = "Any",
                Price = 100,
                Duration = 30
            };

            var result = await _service.UpdateSubscriptionPackageAsync("notfound", request);

            Assert.False(result.Success);
            Assert.Equal("Invalid subscriptionpackageId: SubscriptionPackage does not exist.", result.Message);
        }

        [Fact]
        public async Task UpdateSubscriptionPackageStatusAsync_ShouldToggleStatus()
        {
            var id = "321";
            var entity = new SubscriptionPackage { Id = id, Status = "Active" };

            _mockRepo.Setup(x => x.GetSubscriptionPackageById(id)).ReturnsAsync(entity);
            _mockRepo.Setup(x => x.Update(It.IsAny<SubscriptionPackage>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(x => x.Map<SubscriptionPackageGetAllResponse>(It.IsAny<SubscriptionPackage>()))
                       .Returns(new SubscriptionPackageGetAllResponse { PackageName = "Any" });

            var result = await _service.UpdateSubscriptionPackageStatusAsync(id);

            Assert.True(result.Success);
            Assert.Equal("Archived", entity.Status); // Should be toggled
        }

        #endregion
    }
}
