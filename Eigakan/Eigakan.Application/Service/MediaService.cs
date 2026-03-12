
using Aspose.Pdf.Operators;
using AutoMapper;
using DocumentFormat.OpenXml.Office2019.Excel.ThreadedComments;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Media;
using Eigakan.Domain.Request.Person;
using Eigakan.Domain.Response.Media;
using Eigakan.Domain.Response.Movie;
using Eigakan.Domain.Response.Person;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Eigakan.Application.Service
{
	public class MediaService : IMediaService
	{
		private IMediaRepository _mediaRepository;
		private IMoviesRepository _movieRepository;
		private readonly IMapper _mapper;
		private readonly Logger _logger;
		public MediaService(IMediaRepository mediaRepository, IMapper mapper, Logger logger, IMoviesRepository moviesRepository)
		{
			_mediaRepository = mediaRepository;
			_movieRepository = moviesRepository;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Result<Media>> CreateMedia(MediaCreateRequest request)
		{
			try
			{
				var movieId = await _movieRepository.GetMovieById(request.MovieId);
				if (movieId == null) { return new Result<Media> { Success = false, Message = "Not found movieid" }; }
				
				var newGenre = new Media
				{
					Id = Guid.NewGuid().ToString(),
					CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
					MovieId = request.MovieId,
					Name = request.Name,
					Type = request.Type,
					Url = request.Url,
				};
				await _mediaRepository.Insert(newGenre);
				return new Result<Media>
				{ Success = true, Message = "success", Data = newGenre };
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MediaService));
				return new Result<Media> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<List<MediaResponse>>> GetList()
		{
			try
			{

				var personlist = await _mediaRepository.GetList();
				return new Result<List<MediaResponse>>
				{
					Success = true,
					Data = _mapper.Map<List<MediaResponse>>(personlist),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MediaService));
				return new Result<List<MediaResponse>> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<MediaResponse>> GetMediaById(string? id)
		{
			try
			{
				var media = await _mediaRepository.GetMediaById(id);

				if (media == null)
				{
					return new Result<MediaResponse>
					{
						Success = false,
						Message = "Not found",

					};
				}

				return new Result<MediaResponse>
				{
					Success = true,
					Message = "Success",
					Data = _mapper.Map<MediaResponse>(media),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MediaService));
				return new Result<MediaResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<MediaResponse>> UpdateMedia(string? id, MediaUpdateRequest request)
		{
			try
			{
				var media = await _mediaRepository.GetMediaById(id);

				if (media == null)
				{
					return new Result<MediaResponse>
					{
						Success = false,
						Message = "Not found",

					};
				}
				media.Url = request.Url;
				media.Type = request.Type;
				media.Name = request.Name;

				await _mediaRepository.Update(media);
				return new Result<MediaResponse>
				{
					Success = true,
					Data = _mapper.Map<MediaResponse>(media),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MediaService));
				return new Result<MediaResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<Media>> DeleteMedia(string? id)
		{
			try
			{
				var media = await _mediaRepository.GetMediaById(id);
				if (media == null)
				{
					return new Result<Media>
					{
						Success = false,
						Message = "Not found",

					};
				}
				if (await _mediaRepository.DeleteMediaAsync(media.Id))
				{

					return new Result<Media>
					{
						Success = true,
						Message = "Delete success"
					};
				}
				else
				{
					return new Result<Media>
					{
						Success = false,
						Message = "Delete fail"
					};
				}
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MediaService));
				return new Result<Media> { Success = false, Message = ex.Message };
			}
		}
	}
}
