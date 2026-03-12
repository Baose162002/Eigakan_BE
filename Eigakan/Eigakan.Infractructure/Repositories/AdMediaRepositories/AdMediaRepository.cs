using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.AdMediaRepositories
{
    public class AdMediaRepository :GenericBase<AdMedia> ,IAdMediaRepository
    {
        private readonly EigakanDbContext _context;

        public AdMediaRepository(EigakanDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdMedia>> GetList(string? status, int page, int pageSize)
        {
            var slotTime = await Get(
				orderBy: q => q.OrderByDescending(u => u.CreateAt),
				filter: q => (string.IsNullOrEmpty(status) || q.status == status),
                pageIndex: page,
                pageSize: pageSize
            );
            return slotTime.ToList();
        }

		public async Task<List<AdMedia>> GetListActive()
		{
			var slotTime = await Get(u => u.status == "ACTIVE");
			return slotTime.OrderBy(u => Guid.NewGuid()).ToList(); 
		}
        
        public async Task<List<AdMedia>> GetListMediaActive()
        {
            return await _context.AdMedias
                .Include(x => x.AdPurchaseItems) 
                .Include(x => x.adMediaCounts)  
                .Where(x => x.status == "ACTIVE")
                .ToListAsync();
        }


        public async Task<AdMedia> GetAdMediaById(string id)
        {
            return await GetSingle(filter: c => c.Id == id);
        }

        public async Task<bool> DeleteAdMediaAsync(string? Id)
        {
            var slot = await _context.AdMedia.FindAsync(Id);
            if (slot != null)
            {
                _context.AdMedia.Remove(slot);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<List<AdMedia>> GetListMediaByUserId(string mediaId, int page, int pageSize)
        {
            var slotTime = await Get(filter: q => q.Id == mediaId,
                pageIndex: page,
                pageSize: pageSize
            );
            return slotTime.OrderByDescending(u => u.CreateAt).ToList();
        }
        public async Task<List<AdMedia>> GetListMediaStatusExpiredByUserId(string mediaId, int page, int pageSize)
        {
            var slotTime = await Get(
                filter: q => q.Id == mediaId && q.status == "EXPIRED",
                pageIndex: page,
                pageSize: pageSize
            );

            return slotTime.OrderByDescending(u => u.CreateAt).ToList();
        }

        //public async Task<List<AdMedia>> GetListActiveFollowTime()
        //{
        //    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        //    var getTimeOnly = TimeOnly.FromDateTime(now);
        //    Console.WriteLine(getTimeOnly);
        //    var adMediaList = await _context.AdMedias
        //        .Where(m => m.status == "ACTIVE") // optional
        //        .Join(_context.AdPurchaseSlots, media => media.AdPurchaseSlotId, slot => slot.Id, (media, slot) => new { media, slot })
        //        .Join(_context.AdSlotTimes, ms => ms.slot.AdSlotTimeID, time => time.Id, (ms, time) => new { ms.media, ms.slot, time })
        //        .Join(_context.AdSlotTimeRanges, mst => mst.time.AdSlotTimeRangeID, range => range.Id, (mst, range) => new { mst.media, mst.slot, mst.time, range })
        //        .Where(x =>
        //                    (x.range.StartTime <= x.range.EndTime && x.range.StartTime <= getTimeOnly && x.range.EndTime >= getTimeOnly)
        //                    ||
        //                    (x.range.StartTime > x.range.EndTime && (getTimeOnly >= x.range.StartTime || getTimeOnly <= x.range.EndTime))
        //                )
        //        .Select(x => x.media)
        //        .ToListAsync();
        //    return adMediaList;
        //}
    }
}
