using Microsoft.EntityFrameworkCore;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;

namespace Eigakan.Infractructure.Repositories.NewsRepositories
{
    public class NewsRepository : GenericBase<News>, INewsRepository
    {
        private readonly EigakanDbContext _context;

        public NewsRepository(EigakanDbContext context)
        {
            _context = context;
        }

        public async Task<List<News>> GetList()
        {
            return await _context.News
                .Include(n => n.User)
                .ToListAsync();
        }

        public async Task<News> GetNewsById(string id)
        {
            return await _context.News
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<News>> GetNewsByUserId(string userId)
        {
            return await _context.News
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreateDate)
                .ToListAsync();
        }
    }
}