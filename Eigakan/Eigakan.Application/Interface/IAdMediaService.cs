using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdMedia;
using Eigakan.Domain.Response.AdMediaCount;
using Eigakan.Domain.Response.AdMediaResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IAdMediaService
    {
        Task<Result<List<AdMedia>>> GetAllListAdMedia(string? status, int page, int pageSize);
        Task<Result<AdMedia>> GetById(string? id);
        Task<Result<AdMedia>> Delete(string? id);
        Task<Result<AdMedia>> AdMediaApprovedStatus(AdMediaApprovedStatus request);
        Task<Result<AdMedia>> AdMediaRejectedStatus(AdMediaRejectedRequest request);
        Task<Result<List<AdMedia>>> GetAllListAdMediaActive();
        //Task<Result<List<AdMedia>>> GetListActiveFollowTime();
        Task<List<AdMediaWithPositionDto>> GetAdMediaWithPositionsAsync(string movieId);

        Task<Result<List<AdMediaGetAllResponse>>> GetMediaByUserIdAsync(int page, int pageSize);
        Task<Result<List<AdMediaGetAllResponse>>> GetMediaStatusEXpiredByUserIdAsync(int page, int pageSize);
    }
}
