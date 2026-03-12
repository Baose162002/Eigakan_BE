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

namespace Eigakan.Infractructure.Repositories.UserRoomRepositories
{
    public class UserRoomRepository : MongoGenericRepository<UserRoom>, IUserRoomRepository
    {
        public UserRoomRepository(MongoDbContext dbContext) : base(dbContext.Database, "UserRooms") { }
   

        public async Task<UserRoom> FindOneAsync(FilterDefinition<UserRoom> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(UserRoom userRoom)
        {
            await _collection.InsertOneAsync(userRoom);
        }

        public async Task UpdateOneAsync(FilterDefinition<UserRoom> filter, UpdateDefinition<UserRoom> update)
        {
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string userId, string roomId)
        {
            await _collection.DeleteOneAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
        }
        public async Task DeleteUserAsync(string id)
        {
            await _collection.DeleteOneAsync(ur => ur.Id == id);
        }
        public async Task<List<UserRoom>> GetUsersInRoomAsync(string roomId)
        {
            return await _collection.Find(ur => ur.RoomId == roomId).ToListAsync();
        }

	}

}
