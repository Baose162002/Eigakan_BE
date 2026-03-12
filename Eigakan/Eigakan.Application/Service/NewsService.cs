using AutoMapper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.News;
using Eigakan.Domain.Response.News;
using Microsoft.Extensions.Logging;
using Eigakan.Domain.Enum;

namespace Eigakan.Application.Service
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsService> _logger;
        private readonly IUserRepository _userRepository;

        public NewsService(
            INewsRepository newsRepository,
            IMapper mapper,
            ILogger<NewsService> logger,
            IUserRepository userRepository)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<Result<List<NewsResponse>>> GetList()
        {
            try
            {
                var news = await _newsRepository.GetList();
                var newsResponses = _mapper.Map<List<NewsResponse>>(news);

                return new Result<List<NewsResponse>>
                {
                    Success = true,
                    Message = "Get list successfully",
                    Data = newsResponses
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news list: {Message}", ex.Message);
                return new Result<List<NewsResponse>>
                {
                    Success = false,
                    Message = $"Failed to get news list: {ex.Message}"
                };
            }
        }

        public async Task<Result<NewsResponse>> GetNewsById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "News ID cannot be empty"
                    };
                }

                var news = await _newsRepository.GetNewsById(id);
                if (news == null)
                    return new Result<NewsResponse> { Success = false, Message = "News not found" };

                var newsResponse = _mapper.Map<NewsResponse>(news);
                return new Result<NewsResponse>
                {
                    Success = true,
                    Message = "Get news successfully",
                    Data = newsResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by ID {Id}: {Message}", id, ex.Message);
                return new Result<NewsResponse>
                {
                    Success = false,
                    Message = $"Failed to get news: {ex.Message}"
                };
            }
        }

        public async Task<Result<NewsResponse>> CreateNews(CreateNewsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    };
                }

                if (string.IsNullOrEmpty(request.Title))
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "Title is required"
                    };
                }

                if (string.IsNullOrEmpty(request.Content))
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "Content is required"
                    };
                }

                // Validate userId
                var user = await _userRepository.GetUserById(request.UserId);
                if (user == null)
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "Invalid UserId. User not found."
                    };
                }

                var news = new News
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = request.Title,
                    Content = request.Content,
                    Picture = request.Picture,
                    Url = request.Url,
                    CreateDate = DateTime.Now,
                    Status = "Active",
                    UserId = request.UserId
                };

                await _newsRepository.Insert(news);
                var newsWithUser = await _newsRepository.GetNewsById(news.Id);
                var newsResponse = _mapper.Map<NewsResponse>(newsWithUser);

                return new Result<NewsResponse>
                {
                    Success = true,
                    Message = "Create news successfully",
                    Data = newsResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news: {Message}", ex.Message);
                return new Result<NewsResponse>
                {
                    Success = false,
                    Message = $"Failed to create news: {ex.Message}"
                };
            }
        }

        public async Task<Result<NewsResponse>> UpdateNews(string id, UpdateNewsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "News ID cannot be empty"
                    };
                }

                if (request == null)
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    };
                }

                var existingNews = await _newsRepository.GetNewsById(id);
                if (existingNews == null)
                    return new Result<NewsResponse> { Success = false, Message = "News not found" };

                if (!string.IsNullOrEmpty(request.Status)
                    && !Enum.TryParse<NewsStatus>(request.Status, out _))
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = $"Invalid status value. Allowed values are: {string.Join(", ", Enum.GetNames<NewsStatus>())}"
                    };
                }

                existingNews.Title = request.Title ?? existingNews.Title;
                existingNews.Content = request.Content ?? existingNews.Content;
                existingNews.Picture = request.Picture ?? existingNews.Picture;
                existingNews.Url = request.Url ?? existingNews.Url;
                existingNews.Status = request.Status ?? existingNews.Status;

                await _newsRepository.Update(existingNews);
                var newsResponse = _mapper.Map<NewsResponse>(existingNews);

                return new Result<NewsResponse>
                {
                    Success = true,
                    Message = "Update news successfully",
                    Data = newsResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news {Id}: {Message}", id, ex.Message);
                return new Result<NewsResponse>
                {
                    Success = false,
                    Message = $"Failed to update news: {ex.Message}"
                };
            }
        }

        public async Task<Result<NewsResponse>> DeleteNews(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "News ID cannot be empty"
                    };
                }

                var news = await _newsRepository.GetNewsById(id);
                if (news == null)
                    return new Result<NewsResponse> { Success = false, Message = "News not found" };

                if (news.Status == NewsStatus.Deleted.ToString())
                {
                    return new Result<NewsResponse>
                    {
                        Success = false,
                        Message = "News is already deleted"
                    };
                }

                news.Status = NewsStatus.Deleted.ToString();
                await _newsRepository.Update(news);
                var newsResponse = _mapper.Map<NewsResponse>(news);

                return new Result<NewsResponse>
                {
                    Success = true,
                    Message = "Delete news successfully",
                    Data = newsResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news {Id}: {Message}", id, ex.Message);
                return new Result<NewsResponse>
                {
                    Success = false,
                    Message = $"Failed to delete news: {ex.Message}"
                };
            }
        }

        public async Task<Result<List<NewsResponse>>> GetNewsByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new Result<List<NewsResponse>>
                    {
                        Success = false,
                        Message = "User ID cannot be empty"
                    };
                }

                var news = await _newsRepository.GetNewsByUserId(userId);
                var newsResponses = _mapper.Map<List<NewsResponse>>(news);

                return new Result<List<NewsResponse>>
                {
                    Success = true,
                    Message = "Get news by user successfully",
                    Data = newsResponses
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by user ID {UserId}: {Message}", userId, ex.Message);
                return new Result<List<NewsResponse>>
                {
                    Success = false,
                    Message = $"Failed to get news: {ex.Message}"
                };
            }
        }
    }
}