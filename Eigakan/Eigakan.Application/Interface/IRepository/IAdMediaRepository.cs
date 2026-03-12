using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IAdMediaRepository :IGenericRepository<AdMedia>
    {
        Task<AdMedia> GetAdMediaById(string id);
        Task<List<AdMedia>> GetList(string? status, int page, int pageSize);
        Task<bool> DeleteAdMediaAsync(string? Id);
		Task<List<AdMedia>> GetListActive();
        Task<List<AdMedia>> GetListMediaActive();
        Task<List<AdMedia>> GetListMediaByUserId(string mediaId, int page, int pageSize);
        Task<List<AdMedia>> GetListMediaStatusExpiredByUserId(string mediaId, int page, int pageSize);

        //Task<List<AdMedia>> GetListActiveFollowTime();


    }
}
