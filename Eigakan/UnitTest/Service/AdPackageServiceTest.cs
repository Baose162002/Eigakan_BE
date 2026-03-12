using AutoMapper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdPackage;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
namespace UnitTest.Service
{
	public class AdPackageServiceTest
	{
		private readonly Mock<IAdPackageRepository> _adPackageRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IOptions<DiscordWebhookUrls>> _optionsMock;
		private readonly Mock<Webhook> _webhookMock;
		private readonly AdPackageService _adPackageService;

		public AdPackageServiceTest()
		{
			_mapperMock = new Mock<IMapper>();
			_adPackageRepositoryMock = new Mock<IAdPackageRepository>();
			_optionsMock = new Mock<IOptions<DiscordWebhookUrls>>();
			_optionsMock.Setup(o => o.Value).Returns(new DiscordWebhookUrls
			{
				AdminUpdates = "https://discord.com/api/webhooks/1326070733650137139/qG7-RocMHVj0CFlw3vAr2YGE2Ou7QJfOA9jwzL7TgZj0JJTgYkSUBFg_yfWMWPYhjLs-"
			});

			_webhookMock = new Mock<Webhook>(_optionsMock.Object);

			var logger = new Logger(_webhookMock.Object);

			_adPackageService = new AdPackageService(
				_adPackageRepositoryMock.Object,
				logger,
				_mapperMock.Object
			);
		}

		#region create ad package
		[Fact]
		public async Task CreateAdPackage_ShouldReturnError_WhenMinViewGreaterThanMaxView()
		{
			// Arrange
			var request = new AdPackageCreateRequest
			{
				MinView = 10,
				MaxView = 5,
				PackageName = "Test Package",
				PricePerView = 100
			};

			// Act
			var result = await _adPackageService.CreateAdPackage(request);

			// Assert
			Assert.False(result.Success);
			Assert.Equal("MinView cannot be greater than MaxView", result.Message);
		}

		[Fact]
		public async Task CreateAdPackage_ShouldReturnError_WhenOverlappingPackagesExist()
		{
			// Arrange
			var request = new AdPackageCreateRequest
			{
				MinView = 10,
				MaxView = 20,
				PackageName = "Test Package",
				PricePerView = 100
			};

			_adPackageRepositoryMock
				.Setup(repo => repo.GetAdPackageByMinMax(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<AdPackage> { new AdPackage() });

			// Act
			var result = await _adPackageService.CreateAdPackage(request);

			// Assert
			Assert.False(result.Success);
			Assert.Equal("There's already an active package that overlaps with this view range.", result.Message);
		}

		[Fact]
		public async Task CreateAdPackage_ShouldReturnSuccess_WhenPackageCreated()
		{
			// Arrange
			var request = new AdPackageCreateRequest
			{
				MinView = 10,
				MaxView = 20,
				PackageName = "Test Package",
				PricePerView = 100
			};

			_adPackageRepositoryMock
				.Setup(repo => repo.GetAdPackageByMinMax(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<AdPackage>());

			_adPackageRepositoryMock
				.Setup(repo => repo.Insert(It.IsAny<AdPackage>()))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _adPackageService.CreateAdPackage(request);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Ad package created successfully", result.Message);
		}
		#endregion

		#region update ad package

		[Fact]
		public async Task UpdateAdPackage_ShouldReturnError_WhenMinViewGreaterThanMaxView()
		{
			// Arrange
			var request = new AdPackageUpdateRequest
			{
				MinView = 10,
				MaxView = 5,
				PackageName = "Updated Package",
				PricePerView = 200
			};

			// Act
			var result = await _adPackageService.UpdateAdPackage("some-id", request);

			// Assert
			Assert.False(result.Success);
			Assert.Equal("MinView cannot be greater than MaxView", result.Message);
		}


		[Fact]
		public async Task UpdateAdPackage_ShouldReturnError_WhenAdPackageNotFound()
		{
			// Arrange
			var request = new AdPackageUpdateRequest
			{
				MinView = 10,
				MaxView = 20,
				PackageName = "Updated Package",
				PricePerView = 200
			};

			_adPackageRepositoryMock
				.Setup(repo => repo.GetAdPackageById(It.IsAny<string>()))
				.ReturnsAsync((AdPackage)null);

			// Act
			var result = await _adPackageService.UpdateAdPackage("invalidid", request);

			// Assert
			Assert.False(result.Success);
			Assert.Equal("Ad package not found", result.Message);
		}

		[Fact]
		public async Task UpdateAdPackage_ShouldReturnSuccess_WhenPackageUpdated()
		{
			// Arrange
			var request = new AdPackageUpdateRequest
			{
				MinView = 10,
				MaxView = 20,
				PackageName = "Updated Package",
				PricePerView = 200
			};

			var existingPackage = new AdPackage
			{
				Id = "some-id",
				PackageName = "Old Package",
				MinView = 5,
				MaxView = 15,
				PricePerView = 100
			};

			_adPackageRepositoryMock
				.Setup(repo => repo.GetAdPackageByMinMax(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(new List<AdPackage>());

			_adPackageRepositoryMock
				.Setup(repo => repo.GetAdPackageById(It.IsAny<string>()))
				.ReturnsAsync(existingPackage);

			_adPackageRepositoryMock
				.Setup(repo => repo.Update(It.IsAny<AdPackage>()))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _adPackageService.UpdateAdPackage("some-id", request);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Ad package updated successfully", result.Message);
		}

		#endregion
	}
}
