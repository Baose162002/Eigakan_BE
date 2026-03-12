using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.ContractRequest;
using Eigakan.Domain.Response.ContractResponse;
using Eigakan.Domain.Response.Movie;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{


    public class ContractServiceTest
    {
        private readonly Mock<IContractRepository> _contractRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IMoviesRepository> _moviesRepositoryMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<ContractService>> _loggerMock = new();
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private readonly Mock<IAmazonS3> _amazonS3Mock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly Mock<ICacheService> _cacheServiceMock = new();

        private readonly ContractService _contractService;

        public ContractServiceTest()
        {
            // Mock configuration for AWS
            _configurationMock.Setup(c => c["AWS:BucketName"]).Returns("your-bucket-name");
            _configurationMock.Setup(c => c["AWS:AccessKey"]).Returns("your-access-key");
            _configurationMock.Setup(c => c["AWS:SecretKey"]).Returns("your-secret-key");
            _configurationMock.Setup(c => c["AWS:Region"]).Returns("us-east-1");

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            // Mock AWS S3 (instead of real S3 Client)
            _amazonS3Mock.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK }));

            // Mock Lazy<IAmazonS3> to return the mocked S3 client
            var lazyAmazonS3Mock = new Lazy<IAmazonS3>(() => _amazonS3Mock.Object);

            // Initialize ContractService once here
            _contractService = new ContractService(
                _contractRepositoryMock.Object,
                _userRepositoryMock.Object,
                _moviesRepositoryMock.Object,
                _configurationMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                httpClient,
                lazyAmazonS3Mock,
                _httpContextAccessorMock.Object,
                _emailServiceMock.Object,
                _cacheServiceMock.Object
            );
        }

       
      

        [Fact]
        public async Task AcceptedContract_ShouldReturnSuccess_WhenValid()
        {
            var request = new AcceptContractRequest { Id = "123", SignToken = "valid-token" };
            var contract = new Contract { Id = "123", SignToken = "valid-token", Status = "PENDING", MovieId = "movie123" };
            var movie = new Movie { Id = "movie123" };

            _contractRepositoryMock.Setup(x => x.GetContractById(request.Id)).ReturnsAsync(contract);
            _moviesRepositoryMock.Setup(x => x.GetMovieById(contract.MovieId)).ReturnsAsync(movie);

            var result = await _contractService.AcceptedContract(request);

            Assert.True(result.Success);
            Assert.Equal("Update status successfull", result.Message);
        }

        [Fact]
        public async Task AcceptedContract_ShouldReturnError_WhenIdNotFound()
        {
            _contractRepositoryMock.Setup(x => x.GetContractById(It.IsAny<string>())).ReturnsAsync((Contract)null);

            var result = await _contractService.AcceptedContract(new AcceptContractRequest { Id = "notfound" });

            Assert.False(result.Success);
            Assert.Equal("Id does not exist", result.Message);
        }

        [Fact]
        public async Task AcceptedContract_ShouldReturnError_WhenInvalidSignToken()
        {
            var contract = new Contract { Id = "123", SignToken = "expected-token" };
            _contractRepositoryMock.Setup(x => x.GetContractById(contract.Id)).ReturnsAsync(contract);

            var result = await _contractService.AcceptedContract(new AcceptContractRequest { Id = contract.Id, SignToken = "wrong-token" });

            Assert.False(result.Success);
            Assert.Equal("Invalid SignToken", result.Message);
        }

        [Fact]
        public async Task DeniedContract_ShouldReturnSuccess_WhenValid()
        {
            var request = new DeniedContractRequest { Id = "123", ReasonForDenying = "reason" };
            var contract = new Contract { Id = "123", Status = "PENDING" };

            _contractRepositoryMock.Setup(x => x.GetContractById(request.Id)).ReturnsAsync(contract);

            var result = await _contractService.DeniedContract(request);

            Assert.True(result.Success);
            Assert.Equal("Update status successfull", result.Message);
        }

        [Fact]
        public async Task DeniedContract_ShouldReturnError_WhenIdNotFound()
        {
            _contractRepositoryMock.Setup(x => x.GetContractById(It.IsAny<string>())).ReturnsAsync((Contract)null);

            var result = await _contractService.DeniedContract(new DeniedContractRequest { Id = "notfound" });

            Assert.False(result.Success);
            Assert.Equal("Id does not exist", result.Message);
        }

        [Fact]
        public async Task DeniedContract_ShouldReturnError_WhenAlreadyFinalized()
        {
            var contract = new Contract { Id = "123", Status = ContractStatusEnum.SIGNED.ToString() };
            _contractRepositoryMock.Setup(x => x.GetContractById(contract.Id)).ReturnsAsync(contract);

            var result = await _contractService.DeniedContract(new DeniedContractRequest { Id = contract.Id });

            Assert.False(result.Success);
            Assert.Equal("Can not update this register", result.Message);
        }
    }
}
