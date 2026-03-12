using Eigakan.Application.Helper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.DomainMongoDB.Base;
using Eigakan.DomainMongoDB.Models;
using Eigakan.Infractructure.Repositories.MongoGenericRepositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.RoomRepositories
{
    public class RoomRepository : MongoGenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(MongoDbContext dbContext) : base(dbContext.Database, "Rooms") { }

        public async Task<List<Room>> GetActiveRoomsAsync()
        {
            return await FindAsync(Builders<Room>.Filter.Eq(r => r.IsActive, true));
        }
        
        public async Task<Room> GetByIdAsync(string id)
        {
            return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(Room room)
        {
            await _collection.InsertOneAsync(room);
        }

        public async Task EndRoomAsync(string roomId)
        {
            var update = Builders<Room>.Update
                .Set(r => r.IsActive, false)
                .Set(r => r.Status, "Ended");

            await UpdateOneAsync(Builders<Room>.Filter.Eq("_id", roomId), update);
        }
        
        public async Task<Room> GetRoomIdIfHostAsync(string userId)
        {

            var room = await _collection
                .Find(r => r.HostId == userId && r.IsActive && r.Status == "Active")
                .FirstOrDefaultAsync();

            return room;
        }
        
        public async Task<List<Room>> GetExpiredRoomsAsync(DateTime now)
        {
            var filter = Builders<Room>.Filter.Lt(r => r.EndTime, now) & Builders<Room>.Filter.Eq(r => r.Status, "Active");
            return await _collection.Find(filter).ToListAsync();
        }
        
        public async Task UpdateAsync(Room room)
        {
            var filter = Builders<Room>.Filter.Eq(r => r.Id, room.Id);
            await _collection.ReplaceOneAsync(filter, room);
        }
        
        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Room>> GetRoomsByUserIdAsync(string userId)
        {
            return await _collection.Find(room => room.HostId == userId).ToListAsync();
        }
    }

}
