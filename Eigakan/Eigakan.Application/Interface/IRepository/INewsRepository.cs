using Eigakan.Domain.Models;

namespace Eigakan.Application.Interface.IRepository
{
    public interface INewsRepository : IGenericRepository<News>
    {
        Task<List<News>> GetList();
        Task<News> GetNewsById(string id);
        Task<List<News>> GetNewsByUserId(string userId);
    }
}