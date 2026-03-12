using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.AdMediaRepositories
{
	public class AdMediaCountRepository : GenericBase<AdMediaCount>, IAdMediaCountRepository
	{
		private readonly EigakanDbContext _context;

		public AdMediaCountRepository(EigakanDbContext context)
		{
			_context = context;
		}

		public async Task<AdMediaCount> GetAdMediaCountByAdMediaId(string adMediaId)
		{
			return await _context.AdMediaCounts
				.Include(a => a.AdMedia)
				.FirstOrDefaultAsync(u => u.AdMedia.Id == adMediaId);
		}

		public async Task<AdMediaCount> CheckCountByAdMediaDate(string adMediaId, string movieId, DateOnly dateTime)
		{
			return await _context.AdMediaCounts
				.Include(a => a.AdMedia)
				.FirstOrDefaultAsync(u => u.AdMedia.Id == adMediaId && u.AdMediaId == movieId && u.ViewDate == dateTime);
		}

		public async Task<List<AdMediaCount>> GetAllAdMediaCountByAdMediaId(string adMediaId)
		{
			return await _context.AdMediaCounts
				.Include(a => a.AdMedia)
				.Where(u => u.AdMedia.Id == adMediaId)
				.ToListAsync();
		}

		public async Task InsertWithAdMedia(AdMediaCount adMediaCount, string adMediaId, string? movieId)
		{
			var adMedia = await _context.AdMedias.FindAsync(adMediaId);
			adMediaCount.AdMediaId = movieId;
			adMediaCount.AdMedia = adMedia;
			_context.AdMediaCounts.Add(adMediaCount);
			await _context.SaveChangesAsync();
		}

		public async Task<AdMediaCount> UpdateViewCount(string adMediaId, string movieId, DateOnly dateTime)
		{
			var count = await _context.AdMediaCounts
				.Include(a => a.AdMedia)
				.FirstOrDefaultAsync(u => u.AdMedia.Id == adMediaId && u.AdMediaId == movieId && u.ViewDate == dateTime);

			if (count != null)
			{
				await _context.SaveChangesAsync();
			}

			return count;
		}

        public async Task<AdMediaCount?> GetByMediaIdAndDate(string mediaId, DateOnly date)
        {
            return await _context.AdMediaCounts
                .FirstOrDefaultAsync(x => x.AdMediaId == mediaId && x.ViewDate == date);
        }
        public async Task<DateOnly?> GetLastViewDate(string mediaId)
        {
            return await _context.AdMediaCounts
                .Where(x => x.AdMediaId == mediaId)
                .OrderByDescending(x => x.ViewDate)
                .Select(x => (DateOnly?)x.ViewDate)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasAnyViewCount(string mediaId)
        {
            return await _context.AdMediaCounts.AnyAsync(x => x.AdMediaId == mediaId);
        }

    }
} 