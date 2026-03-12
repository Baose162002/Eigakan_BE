using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Comment;
using Eigakan.Domain.Request.MovieRating;
using Eigakan.Domain.Response.Genre;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class MovieRatingService :IMovieRatingService
    {
        private readonly IMovieRatingRepository _movieRatingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoviesRepository _moviesRepository;
        private readonly Logger _logger;

        public MovieRatingService(IMovieRatingRepository movieRatingRepository, IMoviesRepository moviesRepository,
                                  IHttpContextAccessor httpContextAccessor, Logger logger)
        {
            _moviesRepository=moviesRepository;
            _movieRatingRepository = movieRatingRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<Result<MovieRating>> Rating(MovieRatingCreateRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);

                var movie = await _moviesRepository.GetMovieById(request.MovieId);
                if (movie == null)
                {
                    return new Result<MovieRating>
                    {
                        Success = false,
                        Message = "Not found movie by movieId",


                    };
                }
                var ratingExisted = await _movieRatingRepository.GetMovieRatingByUserId(userId.Value,request.MovieId);


                if (ratingExisted != null)
                {
                   

                    return new Result<MovieRating>
                    {
                        Success = true,
                        Message = "You are already rated this movie.",
                        Data = await _movieRatingRepository.GetMovieRatingById(ratingExisted.Id)

                    };
                 }
               
                var rating = new MovieRating
                {
                    Id = Guid.NewGuid().ToString(),
                    Stars = request.Stars,
                    UserId = userId.Value.ToString(),
                    CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    MovieId = request.MovieId
                };

                await _movieRatingRepository.Insert(rating);

                return new Result<MovieRating>
                {
                    Success = true,
                    Message = "Create success!",
                    Data =await _movieRatingRepository.GetMovieRatingById(rating.Id)

                };
            }
            catch (Exception ex)
            {
                // await _logger.LogError(ex, nameof(CommentService));
                // _logger.LogError(ex, "Error while inserting movie.");
                return new Result<MovieRating>
                {
                    Success = false,
                    Message = $"ERROR: {ex.Message}"
                };
            }

        }
        public async Task<Result<MovieRating>> Update(string? id, MovieRatingUpdateRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
                var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

                var rating = await _movieRatingRepository.GetMovieRatingById(id);
                if (userId.Value != rating.UserId)
                {
                    return new Result<MovieRating>
                    {
                        Success = false,
                        Message = "YOU DON'T HAVE AUTHORIZATION"
                    };
                }
                rating.Stars = request.Stars;
                await _movieRatingRepository.Update(rating);

                return new Result<MovieRating>
                {
                    Success = true,
                    Message = "Update success",
                    Data = await _movieRatingRepository.GetMovieRatingById(rating.Id)

                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(CommentService));
                _logger.LogError(ex, "Error while inserting movie.");
                return new Result<MovieRating>
                {
                    Success = false,
                    Message = "ERROR!"
                };
            }

        }
        public async Task<Result<MovieRating>> GetMovieRatingByLogin(string movieId)
        {
            try
            {
				var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
                if(userId.Value == null)
                {
                    return new Result<MovieRating>
                    {
                        Message = "id Not Found",
                        Success = false,
                    };
                }
				
                var rating = await _movieRatingRepository.GetMovieRatingByLogin(userId.Value, movieId);
                if (rating == null)
                {
                    return new Result<MovieRating>
                    {
                        Success = false,
                        Message = "Not found",
                        Data = null
                    };
                }
                return new Result<MovieRating>
                {
                    Success = true,
                    Data = rating
                };
            }
            catch (Exception ex)
            {
                return new Result<MovieRating>
                {
                    Success = false,
                    Message = ex.Message                
                };
            }
        }
        public async Task<Result<MovieRating>> Delete(string? id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
                var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

                var rating = await _movieRatingRepository.GetMovieRatingById(id);
                if (rating == null)
                {
                    return new Result<MovieRating>
                    {
                        Success = false,
                        Message = "RATING NOT FOUND"
                    };
                }

                // Chỉ cho phép Manager hoặc chính chủ xóa bình luận
                if (roleClaim.Value != "Manager" && userId.Value != rating.UserId)
                {
                    return new Result<MovieRating>
                    {
                        Success = false,
                        Message = "YOU DON'T HAVE AUTHORIZATION"
                    };
                }

                // Thực hiện xóa
                bool isDeleted = await _movieRatingRepository.DeleteMovieRatingAsync(rating.Id);
                if (isDeleted)
                {
                    return new Result<MovieRating>
                    {
                        Success = true,
                        Message = "Delete success"
                    };
                }
                else
                {
                    return new Result<MovieRating>
                    {
                        Success = false,
                        Message = "Delete failed"
                    };
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(MovieRatingService));
                _logger.LogError(ex, "Error while inserting movie.");
                return new Result<MovieRating>
                {
                    Success = false,
                    Message = "ERROR!"
                };
            }

        }


    }
}
