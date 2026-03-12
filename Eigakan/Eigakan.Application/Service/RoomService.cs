using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.ContractResponse;
using Eigakan.DomainMongoDB.Models;
using Eigakan.DomainMongoDB.Request;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMoviesRepository _movieRepository;
        private readonly IUserRoomRepository _userRoomRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public RoomService(IRoomRepository roomRepository, IMapper mapper, IMoviesRepository movieRepository, IUserRoomRepository userRoomRepository, IHttpContextAccessor httpContextAccessor)
        {
            _roomRepository = roomRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
            _userRoomRepository = userRoomRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Room>> GetActiveRoomsAsync() => await _roomRepository.GetActiveRoomsAsync();

		public async Task<Result<Room>> GetRoomByIdAsync(string id)
		{
			var exsitingRoom = await _roomRepository.GetByIdAsync(id);
			if (exsitingRoom == null)
			{
				return new Result<Room> { Success = false, Message = "Not found room" };
			}

			return new Result<Room> { Success = true, Data = exsitingRoom };

		}

        public async Task<string> GetRoomLinkAsync(string roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room does not exist!");
            return $"http://localhost:5173/room/{roomId}";
        }

        public async Task<(bool IsInRoom, bool IsHost)> CheckUserInRoomAsync(string roomId)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User not found.");
            }

            var userRoom = await _userRoomRepository.FindOneAsync(
                Builders<UserRoom>.Filter.Where(ur => ur.RoomId == roomId && ur.UserId == userId));

            if (userRoom != null)
            {
                return (true, userRoom.IsHost);
            }

            return (false, false);
        }
        
        public async Task<List<UserRoom>> GetUsersInRoomAsync(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room ID cannot be left blank.");
            }

            return await _userRoomRepository.GetUsersInRoomAsync(roomId);
        }

		public async Task<Room?> GetHostRoomIdAsync()
		{
			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;

			if (string.IsNullOrEmpty(userId))
			{
				return null; 
			}

			var room = await _roomRepository.GetRoomIdIfHostAsync(userId);
			return room; 
		}

		public async Task EndExpiredRoomsAsync()
        {
            var now = DateTime.UtcNow.AddHours(7); 
            var expiredRooms = await _roomRepository.GetExpiredRoomsAsync(now);

            foreach (var room in expiredRooms)
            {
                room.IsActive = false;
                room.Status = "Ended"; 
                await _roomRepository.UpdateAsync(room);
                Console.WriteLine($"[RoomService] Room {room.Id} has been set to 'Ended'.");
            }
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllRoomsAsync();
        }

        public async Task<List<Room>> GetRoomsByUserIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not found.");
            return await _roomRepository.GetRoomsByUserIdAsync(userId);
        }

		public async Task<Room> CreateRoomAsync(RoomCreateRequest roomRequest)
		{

			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
			if (string.IsNullOrEmpty(userId))
				throw new Exception("User not found.");
			var existingRoom = await _roomRepository.GetRoomIdIfHostAsync(userId);
			if (existingRoom != null)
			{
				if (existingRoom.Status == "Active")
				{
					throw new Exception("You already have an active room. You cannot create another room until it ends.");
				}
			}

			var movie = await _movieRepository.GetMovieById(roomRequest.MovieID);
			if (movie == null)
				throw new Exception("Movie information not found!");

			var now = DateTime.UtcNow.AddHours(7);

			var room = _mapper.Map<Room>(roomRequest);
			room.Id = Guid.NewGuid().ToString();
			room.CreateDate = now;
			room.StartTime = now;
			room.EndTime = now.AddMinutes((double)(movie.Duration + 10));
			room.Status = "Active";
			room.HostId = userId;

			await _roomRepository.InsertAsync(room);

			var userRoom = new UserRoom
			{
				Id = Guid.NewGuid().ToString(),
				UserId = room.HostId,
				RoomId = room.Id,
				JoinedAt = now,
				IsHost = true
			};

			await _userRoomRepository.InsertAsync(userRoom);


			return room;

		}

		public async Task<List<UserRoom>> JoinRoomAsync(JoinRoomRequest request)
		{

			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				throw new Exception("User not found.");
			}

			var room = await _roomRepository.GetByIdAsync(request.RoomId);
			if (room == null)
			{
				throw new Exception("Room does not exist!");
			}
			if (!room.IsActive && room.Status == "Ended")
			{
				throw new Exception("This room is no longer available to join!");
			}


			var usersInRoom = await _userRoomRepository.GetUsersInRoomAsync(request.RoomId);
			
			bool isHost = room.HostId == userId;

			if (!isHost && usersInRoom.Count >= 5)
			{
				throw new Exception("The room is full and cannot be joined!");
			}

			var existingUser = await _userRoomRepository.FindOneAsync(
				Builders<UserRoom>.Filter.Where(ur => ur.RoomId == request.RoomId && ur.UserId == userId));

			if (existingUser != null)
			{
				throw new Exception("The user has already joined this room!");
			}

			var userRoom = new UserRoom
			{
				Id = Guid.NewGuid().ToString(),
				UserId = userId,
				RoomId = request.RoomId,
				JoinedAt = DateTime.UtcNow,
				IsHost = isHost
			};

			await _userRoomRepository.InsertAsync(userRoom);

			return await _userRoomRepository.GetUsersInRoomAsync(request.RoomId);

		}

		public async Task<bool> LeaveRoomAsync(string roomId)
		{
			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				throw new Exception("User not found.");
			}

			var userRoom = await _userRoomRepository.FindOneAsync(
				Builders<UserRoom>.Filter.Where(ur => ur.RoomId == roomId && ur.UserId == userId));

			if (userRoom == null)
			{
				return false; 
			}

			if (userRoom != null)
			{
				await _userRoomRepository.DeleteUserAsync(userRoom.Id);
			}

			bool isRoomEmpty = (await _userRoomRepository.GetUsersInRoomAsync(roomId)).Count == 0;
			if (isRoomEmpty)
			{
				await _roomRepository.EndRoomAsync(roomId);
			}
			
			return true;
		}

		public async Task EndRoomAsync(string roomId)
		{
			await _roomRepository.EndRoomAsync(roomId);
		}
	}
}
