using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.MovieHistory;
using Eigakan.Domain.Request.UserRegisterRequest;
using Eigakan.Domain.Response.Media;
using Eigakan.Domain.Response.Movie;
using Eigakan.Domain.Response.MovieHistory;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class MovieHistoryService : IMovieHistoryService
	{
		private readonly IMovieHistoryRepository _movieHistoryRepository;
		private readonly IMapper _mapper;
		private readonly Logger _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public MovieHistoryService(IMovieHistoryRepository movieHistoryRepository, IMapper mapper, 
								   Logger logger, IHttpContextAccessor httpContextAccessor) 
		{
			_movieHistoryRepository = movieHistoryRepository;
			_mapper = mapper;
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
		}
		
		public async Task<(List<MovieHistoryResponse> movieHistories, int Total)> GetAlMovieHistoryAsync(int page, int pageSize)
		{
			var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);

			var listMovieHistory = await _movieHistoryRepository.GetAllMovieHistoryByLogin(page, pageSize, UserId.Value);

			var listMovieHistoryResponse = _mapper.Map<List<MovieHistoryResponse>>(listMovieHistory);

			foreach (var history in listMovieHistoryResponse)
			{
				if (history.Movies != null && history.Movies.Medias != null)
				{
					history.Movies.Medias = history.Movies.Medias
						.Where(m => m.Type == "POSTER")
						.Select(m => _mapper.Map<MediaShortRespone>(m))
						.ToList();
				}
			}

			var total = await _movieHistoryRepository.CountAllMovieHistoryAsync(UserId.Value);

			return (listMovieHistoryResponse, total);
		}


		public async Task<Result<MovieHistory>> CreateMovieHistory(MovieHistoryCreateRequest movieHistoryCreateRequest)
		{
			try
			{
				var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);

				var checkMovieHistory = await _movieHistoryRepository.GetMovieHistoryByUserMovie(movieHistoryCreateRequest.MovieId, UserId.Value);
				
				if (checkMovieHistory != null)
				{
					checkMovieHistory.CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

					await _movieHistoryRepository.Update(checkMovieHistory);
					return new Result<MovieHistory>
					{
						Success = true,
						Message = "Update movie history successfull!!",
					};
				}

				var newMovieHistory = new MovieHistory()
				{
					Id = Guid.NewGuid().ToString(),
					CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
					MovieId = movieHistoryCreateRequest.MovieId,
					UserId = UserId.Value,					
				};

				await _movieHistoryRepository.Insert(newMovieHistory);
				return new Result<MovieHistory> { Success = true, Message = "Create successfull", Data = newMovieHistory };
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieHistory));
				return new Result<MovieHistory> { Success = false, Message = ex.Message };
			}
		}
	
	}
}
