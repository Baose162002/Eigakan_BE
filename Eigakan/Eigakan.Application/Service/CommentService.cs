using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Comment;
using Eigakan.Domain.Response.Movie;
using Eigakan.Domain.Response.News;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Eigakan.Application.Service
{
    public class CommentService :ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoviesRepository _moviesRepository;
        private readonly Logger _logger;
        public CommentService(ICommentRepository commentRepository, IMoviesRepository moviesRepository
            , IHttpContextAccessor httpContextAccessor,Logger logger)
        {
            _moviesRepository= moviesRepository;
            _commentRepository = commentRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<Result<Comment>> Create(CommentCreateRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
                var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
               
                if (roleClaim == null || !(roleClaim.Value == "ADMIN" || roleClaim.Value == "VIP MEMBER"))
                {
                    return new Result<Comment>
                    {
                        Success = false,
                        Message = "Unauthorized! Only Admin or VIPMember can comment."
                    };
                }
                var movie = await _moviesRepository.GetMovieById(request.MovieId);
                if (movie == null)
                {


                    return new Result<Comment>
                    {
                        Success = false,
                        Message = "Not found movie by movieId",


                    };
                }
                var comment = new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = request.Content,
                    CreateBy = userId.Value.ToString(),
                    CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    MovieId = request.MovieId
                };

                await _commentRepository.Insert(comment);
            
                return new Result<Comment>
                {
                    Success = true,
                    Message = "Create success!",
                    Data = await _commentRepository.GetCommentById(comment.Id)

                };
            }
            catch (Exception ex)
            {
               // await _logger.LogError(ex, nameof(CommentService));
               // _logger.LogError(ex, "Error while inserting movie.");
                return new Result<Comment>
                {
                    Success = false,
                    Message = "ERROR!"
                };
            }

        }

        public async Task<Result<Comment>> Update(string? id, CommentUpdateRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
                var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

                var comment = await _commentRepository.GetCommentById(id);
                if (userId.Value != comment.CreateBy)
                {
                    return new Result<Comment>
                    {
                        Success = false,
                        Message = "YOU DON'T HAVE AUTHORIZATION"
                    };
                }
                comment.Content = request.Content;
                await _commentRepository.Update(comment);

                return new Result<Comment>
                {
                    Success = true,
                    Message = "Update success",
                    Data = await _commentRepository.GetCommentById(comment.Id)

                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(CommentService));
                _logger.LogError(ex, "Error while inserting movie.");
                return new Result<Comment>
                {
                    Success = false,
                    Message = "ERROR!"
                };
            }

        }

        public async Task<Result<Comment>> Delete(string? id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
                var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

                var comment = await _commentRepository.GetCommentById(id);
                if (comment == null)
                {
                    return new Result<Comment>
                    {
                        Success = false,
                        Message = "COMMENT NOT FOUND"
                    };
                }

                // Chỉ cho phép Manager hoặc chính chủ xóa bình luận
                if (roleClaim.Value!= "MANAGER" && userId.Value != comment.CreateBy)
                {
                    return new Result<Comment>
                    {
                        Success = false,
                        Message = "YOU DON'T HAVE AUTHORIZATION"
                    };
                }

                // Thực hiện xóa
                bool isDeleted = await _commentRepository.DeleteCommentAsync(comment.Id);
                if (isDeleted)
                {
                    return new Result<Comment>
                    {
                        Success = true,
                        Message = "Delete success"
                    };
                }
                else
                {
                    return new Result<Comment>
                    {
                        Success = false,
                        Message = "Delete failed"
                    };
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(CommentService));
                _logger.LogError(ex, "Error while inserting movie.");
                return new Result<Comment>
                {
                    Success = false,
                    Message = "ERROR!"
                };
            }

        }
        
        public async Task<Result<Comment>> GetCommentById(string? id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentById(id);

                if (comment == null)
                {
                    return new Result<Comment>
                    {
                        Success = false,
                        Message = "Not found",
                        Data = null
                    };
                }



                return new Result<Comment>
                {
                    Success = true,
                    Data = comment
                };
            }
            catch (Exception ex)
            {
                return new Result<Comment>
                {
                    Success = false,
                    Message = ex.Message

                };
            }
        }

    }
}
