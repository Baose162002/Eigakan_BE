using Eigakan.Application.Shared.Response;
using Eigakan.DomainMongoDB.Models;
using Eigakan.DomainMongoDB.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IRoomService
    {
        Task<List<Room>> GetActiveRoomsAsync();
        Task<Result<Room>> GetRoomByIdAsync(string id);
		Task<Room> CreateRoomAsync(RoomCreateRequest roomRequest);
        Task EndRoomAsync(string roomId);
        Task<List<UserRoom>> JoinRoomAsync(JoinRoomRequest request);
        Task<string> GetRoomLinkAsync(string roomId);
        Task<(bool IsInRoom, bool IsHost)> CheckUserInRoomAsync(string roomId);
        Task<bool> LeaveRoomAsync(string roomId);
        Task<List<UserRoom>> GetUsersInRoomAsync(string roomId);
        Task<Room> GetHostRoomIdAsync();
        Task EndExpiredRoomsAsync();
        Task<List<Room>> GetAllRoomsAsync();
        Task<List<Room>> GetRoomsByUserIdAsync();
    }
}
