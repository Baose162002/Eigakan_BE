using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.DomainMongoDB.Models;
using Eigakan.DomainMongoDB.Request;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
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
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _roomRepoMock = new();
        private readonly Mock<IMoviesRepository> _movieRepoMock = new();
        private readonly Mock<IUserRoomRepository> _userRoomRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly RoomService _roomService;

        public RoomServiceTests()
        {
            _roomService = new RoomService(
                _roomRepoMock.Object,
                _mapperMock.Object,
                _movieRepoMock.Object,
                _userRoomRepoMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        private void SetupHttpContext(string userId)
        {
            var claims = new List<Claim> { new Claim(MySetting.CLAIM_USERID, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // CreateRoomAsync
        // ───────────────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateRoomAsync_ShouldThrow_WhenUserNotFound()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            await Assert.ThrowsAsync<Exception>(() => _roomService.CreateRoomAsync(new RoomCreateRequest()));
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldThrow_WhenAlreadyHasActiveRoom()
        {
            var userId = "user-active";
            SetupHttpContext(userId);
            _roomRepoMock.Setup(r => r.GetRoomIdIfHostAsync(userId)).ReturnsAsync(new Room { Status = "Active" });

            await Assert.ThrowsAsync<Exception>(() =>
                _roomService.CreateRoomAsync(new RoomCreateRequest { MovieID = "movie-1" }));
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldThrow_WhenMovieNotFound()
        {
            var userId = "user-no-movie";
            SetupHttpContext(userId);
            _roomRepoMock.Setup(r => r.GetRoomIdIfHostAsync(userId)).ReturnsAsync((Room)null);
            _movieRepoMock.Setup(r => r.GetMovieById(It.IsAny<string>())).ReturnsAsync((Movie)null);

            await Assert.ThrowsAsync<Exception>(() =>
                _roomService.CreateRoomAsync(new RoomCreateRequest { MovieID = "not-found" }));
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldCreateRoom_WhenValid()
        {
            var userId = "user-create";
            SetupHttpContext(userId);
            _roomRepoMock.Setup(r => r.GetRoomIdIfHostAsync(userId)).ReturnsAsync((Room)null);
            _movieRepoMock.Setup(m => m.GetMovieById(It.IsAny<string>())).ReturnsAsync(new Movie { Duration = 90 });
            _mapperMock.Setup(m => m.Map<Room>(It.IsAny<RoomCreateRequest>())).Returns(new Room());

            var result = await _roomService.CreateRoomAsync(new RoomCreateRequest { MovieID = "movie-1" });

            Assert.NotNull(result);
            _roomRepoMock.Verify(r => r.InsertAsync(It.IsAny<Room>()), Times.Once);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // JoinRoomAsync
        // ───────────────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task JoinRoomAsync_ShouldThrow_WhenUserNotFound()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            await Assert.ThrowsAsync<Exception>(() => _roomService.JoinRoomAsync(new JoinRoomRequest()));
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldThrow_WhenRoomNotFound()
        {
            var userId = "user-join";
            SetupHttpContext(userId);
            _roomRepoMock.Setup(r => r.GetByIdAsync("room-404")).ReturnsAsync((Room)null);

            await Assert.ThrowsAsync<Exception>(() =>
                _roomService.JoinRoomAsync(new JoinRoomRequest { RoomId = "room-404" }));
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldThrow_WhenRoomEnded()
        {
            var userId = "user-join";
            SetupHttpContext(userId);
            _roomRepoMock.Setup(r => r.GetByIdAsync("room-ended"))
                .ReturnsAsync(new Room { Status = "Ended", IsActive = false });

            await Assert.ThrowsAsync<Exception>(() =>
                _roomService.JoinRoomAsync(new JoinRoomRequest { RoomId = "room-ended" }));
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldThrow_WhenRoomFull()
        {
            var userId = "user-full";
            SetupHttpContext(userId);
            var room = new Room { Id = "room-1", HostId = "host", Status = "Active", IsActive = true };
            _roomRepoMock.Setup(r => r.GetByIdAsync("room-1")).ReturnsAsync(room);
            _userRoomRepoMock.Setup(r => r.GetUsersInRoomAsync("room-1"))
                .ReturnsAsync(Enumerable.Range(1, 5).Select(i => new UserRoom()).ToList());
            _userRoomRepoMock.Setup(r => r.FindOneAsync(It.IsAny<FilterDefinition<UserRoom>>()))
                .ReturnsAsync((UserRoom)null);

            await Assert.ThrowsAsync<Exception>(() =>
                _roomService.JoinRoomAsync(new JoinRoomRequest { RoomId = "room-1" }));
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldThrow_WhenUserAlreadyJoined()
        {
            var userId = "user-already";
            SetupHttpContext(userId);
            var room = new Room { Id = "room-2", HostId = "host", Status = "Active", IsActive = true };
            _roomRepoMock.Setup(r => r.GetByIdAsync("room-2")).ReturnsAsync(room);
            _userRoomRepoMock.Setup(r => r.GetUsersInRoomAsync("room-2")).ReturnsAsync(new List<UserRoom>());
            _userRoomRepoMock.Setup(r => r.FindOneAsync(It.IsAny<FilterDefinition<UserRoom>>()))
                .ReturnsAsync(new UserRoom { Id = "ur-1" });

            await Assert.ThrowsAsync<Exception>(() =>
                _roomService.JoinRoomAsync(new JoinRoomRequest { RoomId = "room-2" }));
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldJoinSuccessfully_WhenValid()
        {
            var userId = "user-ok";
            SetupHttpContext(userId);
            var room = new Room { Id = "room-ok", HostId = "host", Status = "Active", IsActive = true };
            _roomRepoMock.Setup(r => r.GetByIdAsync("room-ok")).ReturnsAsync(room);
            _userRoomRepoMock.Setup(r => r.GetUsersInRoomAsync("room-ok")).ReturnsAsync(new List<UserRoom>());
            _userRoomRepoMock.Setup(r => r.FindOneAsync(It.IsAny<FilterDefinition<UserRoom>>())).ReturnsAsync((UserRoom)null);

            var result = await _roomService.JoinRoomAsync(new JoinRoomRequest { RoomId = "room-ok" });

            Assert.NotNull(result);
            _userRoomRepoMock.Verify(u => u.InsertAsync(It.IsAny<UserRoom>()), Times.Once);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // LeaveRoomAsync
        // ───────────────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task LeaveRoomAsync_ShouldThrow_WhenUserNotFound()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            await Assert.ThrowsAsync<Exception>(() => _roomService.LeaveRoomAsync("room-1"));
        }

        [Fact]
        public async Task LeaveRoomAsync_ShouldReturnFalse_WhenUserNotInRoom()
        {
            SetupHttpContext("user-leave");
            _userRoomRepoMock.Setup(r => r.FindOneAsync(It.IsAny<FilterDefinition<UserRoom>>()))
                .ReturnsAsync((UserRoom)null);

            var result = await _roomService.LeaveRoomAsync("room-x");

            Assert.False(result);
        }

        [Fact]
        public async Task LeaveRoomAsync_ShouldEndRoom_WhenLastUserLeaves()
        {
            var userId = "last-user";
            SetupHttpContext(userId);

            _userRoomRepoMock.Setup(r => r.FindOneAsync(It.IsAny<FilterDefinition<UserRoom>>()))
                .ReturnsAsync(new UserRoom { Id = "ur-last", RoomId = "room-end", UserId = userId });

            _userRoomRepoMock.Setup(r => r.GetUsersInRoomAsync("room-end"))
                .ReturnsAsync(new List<UserRoom>()); // giả lập room trống sau khi xóa

            _userRoomRepoMock.Setup(r => r.DeleteUserAsync("ur-last")).Returns(Task.CompletedTask);

            _roomRepoMock.Setup(r => r.EndRoomAsync("room-end")).Returns(Task.CompletedTask);

            var result = await _roomService.LeaveRoomAsync("room-end");

            Assert.True(result);
            _roomRepoMock.Verify(r => r.EndRoomAsync("room-end"), Times.Once);
        }



        // ───────────────────────────────────────────────────────────────────────────────
        // EndExpiredRoomsAsync
        // ───────────────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task EndExpiredRoomsAsync_ShouldEndExpiredRooms()
        {
            var expiredRooms = new List<Room>
        {
            new Room { Id = "r1", Status = "Active", IsActive = true },
            new Room { Id = "r2", Status = "Active", IsActive = true }
        };
            _roomRepoMock.Setup(r => r.GetExpiredRoomsAsync(It.IsAny<DateTime>())).ReturnsAsync(expiredRooms);

            await _roomService.EndExpiredRoomsAsync();

            _roomRepoMock.Verify(r => r.UpdateAsync(It.Is<Room>(rm => rm.Status == "Ended" && rm.IsActive == false)), Times.Exactly(2));
        }
    }

}
