using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Media;
using Eigakan.Domain.Response.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IMediaService
    {
        Task<Result<Media>> CreateMedia(MediaCreateRequest request);
        Task<Result<List<MediaResponse>>> GetList();
        Task<Result<MediaResponse>> GetMediaById(string? id);
        Task<Result<MediaResponse>> UpdateMedia(string? id, MediaUpdateRequest request);
        Task<Result<Media>> DeleteMedia(string? id);
    }
}
