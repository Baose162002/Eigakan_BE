using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdPurchaseItem;
using Eigakan.Domain.Response.AdPurchaseTransaction;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Service
{
    public class AdPurchaseTransactionServiceTests
    {
        private readonly Mock<IUserWalletRepository> _userWalletRepoMock = new();
        private readonly Mock<IAdPackageRepository> _adPackageRepoMock = new();
        private readonly Mock<IAdMediaRepository> _adMediaRepoMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly Mock<IAdPurchaseItemRepository> _adPurchaseItemRepoMock = new();
        private readonly Mock<IWalletTransactionRepository> _walletTransactionRepoMock = new();
        private readonly Mock<IAdPurchaseTransactionRepository> _adPurchaseTransactionRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private readonly AdPurchaseTranasctionService _service;

        public AdPurchaseTransactionServiceTests()
        {
            _service = new AdPurchaseTranasctionService(
                _userWalletRepoMock.Object,
                _adPackageRepoMock.Object,
                _adMediaRepoMock.Object,
                _httpContextAccessorMock.Object,
                _adPurchaseItemRepoMock.Object,
                _walletTransactionRepoMock.Object,
                _adPurchaseTransactionRepoMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateAdPurchaseAsync_Success_ReturnsSuccessResult()
        {
            // Arrange
            var userId = "user123";
            var userWallet = new UserWallet
            {
                Id = "wallet1",
                UserId = userId,
                Balance = 1000
            };

            var request = new CreateAdPurchaseRequest
            {
                AdPurchaseItems = new List<AdPurchaseItemRequest>
        {
            new AdPurchaseItemRequest
            {
                ViewQuantity = 100,
                NewMedia = new NewAdMediaDto
                {
                    Content = "Test Content",
                    Url = "http://test.com"
                }
            }
        }
            };

            var adPackage = new AdPackage
            {
                Id = "pkg1",
                MinView = 1,
                MaxView = 100,
                PricePerView = 2
            };

            var fakeTransaction = new Mock<IDbContextTransaction>();
            fakeTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            fakeTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            fakeTransaction.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(MySetting.CLAIM_USERID, userId)
            }));
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

            _userWalletRepoMock.Setup(r => r.GetUserWalletById(userId))
                .ReturnsAsync(userWallet);

            _adPackageRepoMock.Setup(r => r.GetFirstAdPackageByViewQuantityAsync(100))
                .ReturnsAsync(adPackage);

            _adMediaRepoMock.Setup(r => r.InsertTransaction(It.IsAny<AdMedia>()))
                .Returns(Task.CompletedTask);
            _adMediaRepoMock.Setup(r => r.SaveChangeTransaction())
                .Returns(Task.CompletedTask);

            _adPurchaseTransactionRepoMock.Setup(r => r.BeginTransactionAsync())
                .ReturnsAsync(fakeTransaction.Object);
            _adPurchaseTransactionRepoMock.Setup(r => r.InsertTransaction(It.IsAny<AdPurchaseTransaction>()))
                .Returns(Task.CompletedTask);
            _adPurchaseTransactionRepoMock.Setup(r => r.SaveChangeTransaction())
                .Returns(Task.CompletedTask);

            _userWalletRepoMock.Setup(r => r.UpdateTransaction(It.IsAny<UserWallet>()))
                .Returns(Task.CompletedTask);
            _userWalletRepoMock.Setup(r => r.SaveChangeTransaction())
                .Returns(Task.CompletedTask);

            _walletTransactionRepoMock.Setup(r => r.InsertTransaction(It.IsAny<WalletTransaction>()))
                .Returns(Task.CompletedTask);
            _walletTransactionRepoMock.Setup(r => r.SaveChangeTransaction())
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<AdPurchaseTransactionGetAllResponse>(It.IsAny<AdPurchaseTransaction>()))
                .Returns(new AdPurchaseTransactionGetAllResponse());

            // Act
            var result = await _service.CreateAdPurchaseAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Purchase successful", result.Message);
            Assert.NotNull(result.Data);

            // Verify that the expected methods were called
            _adMediaRepoMock.Verify(m => m.InsertTransaction(It.IsAny<AdMedia>()), Times.Once);
            _adPurchaseTransactionRepoMock.Verify(m => m.InsertTransaction(It.IsAny<AdPurchaseTransaction>()), Times.Once);
            _userWalletRepoMock.Verify(m => m.UpdateTransaction(It.IsAny<UserWallet>()), Times.Once);
            _walletTransactionRepoMock.Verify(m => m.InsertTransaction(It.IsAny<WalletTransaction>()), Times.Once);
            fakeTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }





        [Fact]
        public async Task CreateAdPurchaseAsync_UserWalletNotFound_ReturnsError()
        {
            // Arrange
            var userId = "user123";
            var request = new CreateAdPurchaseRequest
            {
                AdPurchaseItems = new List<AdPurchaseItemRequest>
        {
            new AdPurchaseItemRequest
            {
                ViewQuantity = 100,
                NewMedia = new NewAdMediaDto { Content = "Test", Url = "http://test.com" }
            }
        }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(MySetting.CLAIM_USERID, userId)
            }));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _userWalletRepoMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync((UserWallet?)null);

            // Act
            var result = await _service.CreateAdPurchaseAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Wallet not found", result.Message);
        }



        [Fact]
        public async Task CreateAdPurchaseAsync_AdPackageNotFound_ReturnsError()
        {
            // Arrange
            var userId = "user123";
            var userWallet = new UserWallet { Id = "wallet1", UserId = userId, Balance = 1000 };

            var request = new CreateAdPurchaseRequest
            {
                AdPurchaseItems = new List<AdPurchaseItemRequest>
        {
            new AdPurchaseItemRequest
            {
                ViewQuantity = 100,
                NewMedia = new NewAdMediaDto { Content = "Test", Url = "http://test.com" }
            }
        }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(MySetting.CLAIM_USERID, userId)
            }));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _userWalletRepoMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync(userWallet);
            _adPackageRepoMock.Setup(r => r.GetFirstAdPackageByViewQuantityAsync(100)).ReturnsAsync((AdPackage?)null);

            // Act
            var result = await _service.CreateAdPurchaseAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No AdPackage available for 100 views", result.Message);
        }



        [Fact]
        public async Task CreateAdPurchaseAsync_InsufficientBalance_ReturnsError()
        {
            // Arrange
            var userId = "user123";
            var userWallet = new UserWallet { Id = "wallet1", UserId = userId, Balance = 50 }; // Chỉ có 50

            var adPackage = new AdPackage
            {
                Id = "pkg1",
                PricePerView = 2,
                MinView = 1,
                MaxView = 100
            };

            var request = new CreateAdPurchaseRequest
            {
                AdPurchaseItems = new List<AdPurchaseItemRequest>
        {
            new AdPurchaseItemRequest
            {
                ViewQuantity = 100,
                NewMedia = new NewAdMediaDto { Content = "Test", Url = "http://test.com" }
            }
        }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(MySetting.CLAIM_USERID, userId)
            }));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _userWalletRepoMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync(userWallet);
            _adPackageRepoMock.Setup(r => r.GetFirstAdPackageByViewQuantityAsync(100)).ReturnsAsync(adPackage);

            // Act
            var result = await _service.CreateAdPurchaseAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Insufficient balance", result.Message);
        }


        [Fact]
        public async Task CreateAdPurchaseAsync_BothMediaIdAndNewMediaProvided_ReturnsError()
        {
            // Arrange
            var userId = "user123";
            var userWallet = new UserWallet { Id = "wallet1", UserId = userId, Balance = 1000 };

            var request = new CreateAdPurchaseRequest
            {
                AdPurchaseItems = new List<AdPurchaseItemRequest>
        {
            new AdPurchaseItemRequest
            {
                ViewQuantity = 50,
                MediaId = "media123",
                NewMedia = new NewAdMediaDto { Content = "Content", Url = "http://test.com" }
            }
        }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(MySetting.CLAIM_USERID, userId)
    }));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _userWalletRepoMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync(userWallet);
            _adPackageRepoMock.Setup(r => r.GetFirstAdPackageByViewQuantityAsync(50)).ReturnsAsync(new AdPackage
            {
                Id = "pkg1",
                PricePerView = 2,
                MinView = 1,
                MaxView = 100
            });

            // Act
            var result = await _service.CreateAdPurchaseAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("You must provide either an existing MediaId or NewMedia", result.Message);
        }



        [Fact]
        public async Task CreateAdPurchaseAsync_NoMediaProvided_ReturnsError()
        {
            // Arrange
            var userId = "user123";
            var userWallet = new UserWallet { Id = "wallet1", UserId = userId, Balance = 1000 };

            var request = new CreateAdPurchaseRequest
            {
                AdPurchaseItems = new List<AdPurchaseItemRequest>
        {
            new AdPurchaseItemRequest
            {
                ViewQuantity = 50
                // Missing MediaId and NewMedia
            }
        }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(MySetting.CLAIM_USERID, userId)
    }));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _userWalletRepoMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync(userWallet);
            _adPackageRepoMock.Setup(r => r.GetFirstAdPackageByViewQuantityAsync(50)).ReturnsAsync(new AdPackage
            {
                Id = "pkg1",
                PricePerView = 2,
                MinView = 1,
                MaxView = 100
            });

            // Act
            var result = await _service.CreateAdPurchaseAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("You must provide either an existing MediaId or NewMedia", result.Message);  
        }


        [Fact]
        public async Task CreateAdPurchaseAsync_MediaIdNotFound_ReturnsError()
        {
            // Arrange
            var userId = "user123";
            var userWallet = new UserWallet { Id = "wallet1", UserId = userId, Balance = 1000 };

            var request = new CreateAdPurchaseRequest
            {
                AdPurchaseItems = new List<AdPurchaseItemRequest>
        {
            new AdPurchaseItemRequest
            {
                ViewQuantity = 50,
                MediaId = "invalidMediaId"
            }
        }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(MySetting.CLAIM_USERID, userId)
    }));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _userWalletRepoMock.Setup(r => r.GetUserWalletById(userId)).ReturnsAsync(userWallet);
            _adPackageRepoMock.Setup(r => r.GetFirstAdPackageByViewQuantityAsync(50)).ReturnsAsync(new AdPackage
            {
                Id = "pkg1",
                PricePerView = 2,
                MinView = 1,
                MaxView = 100
            });

            _adMediaRepoMock.Setup(r => r.GetAdMediaById("invalidMediaId")).ReturnsAsync((AdMedia?)null);

            // Act
            var result = await _service.CreateAdPurchaseAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Media with ID 'invalidMediaId' not found", result.Message);
        }


    }
}