using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.News;
using Eigakan.Domain.Response.News;

namespace Eigakan.Application.Interface
{
    public interface INewsService
    {
        Task<Result<List<NewsResponse>>> GetList();
        Task<Result<NewsResponse>> GetNewsById(string id);
        Task<Result<List<NewsResponse>>> GetNewsByUserId(string userId);
        Task<Result<NewsResponse>> CreateNews(CreateNewsRequest request);
        Task<Result<NewsResponse>> UpdateNews(string id, UpdateNewsRequest request);
        Task<Result<NewsResponse>> DeleteNews(string id);
    }
}