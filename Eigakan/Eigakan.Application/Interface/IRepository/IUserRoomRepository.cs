using Eigakan.DomainMongoDB.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IUserRoomRepository
    {
        Task<UserRoom> FindOneAsync(FilterDefinition<UserRoom> filter);
        Task InsertAsync(UserRoom userRoom);
        Task UpdateOneAsync(FilterDefinition<UserRoom> filter, UpdateDefinition<UserRoom> update);
        Task DeleteAsync(string userId, string roomId);
        Task<List<UserRoom>> GetUsersInRoomAsync(string roomId);
        Task DeleteUserAsync(string id);
    }

}
