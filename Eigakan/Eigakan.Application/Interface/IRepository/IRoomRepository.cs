using Eigakan.DomainMongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetActiveRoomsAsync();
        Task<Room> GetByIdAsync(string id);
        Task InsertAsync(Room room);
        Task EndRoomAsync(string roomId);
        Task<Room?> GetRoomIdIfHostAsync(string userId);
        Task<List<Room>> GetExpiredRoomsAsync(DateTime now);
        Task UpdateAsync(Room room);
        Task<List<Room>> GetAllRoomsAsync();
        Task<List<Room>> GetRoomsByUserIdAsync(string userId);

    }
}
