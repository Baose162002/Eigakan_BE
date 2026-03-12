using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IAdMediaCountRepository : IGenericRepository<AdMediaCount>
	{
		Task<AdMediaCount> GetAdMediaCountByAdMediaId(string adMediaId);
		Task<AdMediaCount> CheckCountByAdMediaDate(string adMediaId, string movieId, DateOnly dateTime);
		Task<List<AdMediaCount>> GetAllAdMediaCountByAdMediaId(string adMediaId);
		Task InsertWithAdMedia(AdMediaCount adMediaCount, string adMediaId, string? movieId);
		Task<AdMediaCount> UpdateViewCount(string adMediaId, string movieId, DateOnly dateTime);
		Task<AdMediaCount?> GetByMediaIdAndDate(string mediaId, DateOnly date);
		Task<DateOnly?> GetLastViewDate(string mediaId);

        Task<bool> HasAnyViewCount(string mediaId);
    }
} 