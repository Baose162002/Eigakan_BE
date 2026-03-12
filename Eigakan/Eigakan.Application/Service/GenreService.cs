using AutoMapper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Genre;
using Eigakan.Domain.Request.Movie;
using Eigakan.Domain.Response.Genre;
using Eigakan.Domain.Response.Movie;
using Eigakan.Domain.Response.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class GenreService : IGenreService
	{
		private readonly IGenreRepository _genreRepository;
		private readonly IMapper _mapper;
		public GenreService(IGenreRepository genreRepository, IMapper mapper)
		{
			_genreRepository = genreRepository;
			_mapper = mapper;
		}

		public async Task<Result<Genre>> CreateGenre(CreateGenreRequest genreRequest)
		{
			try
			{
				if (await _genreRepository.CheckName(genreRequest.Name) == 1)
				{
					return new Result<Genre>
					{ Success = false, Message = "Already have this genre!!!" };

				}
				var newGenre = new Genre
				{
					Id = Guid.NewGuid().ToString(),
					Description = genreRequest.Description,
					Name = genreRequest.Name
				};
				await _genreRepository.Insert(newGenre);
				return new Result<Genre>
				{ Success = true, Message = "success", Data = newGenre };
			}
			catch (Exception ex)
			{
				return new Result<Genre> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<List<GenreListNameResponse>>> GetList()
		{
			try
			{
				var movielist = await _genreRepository.GetList();
				return new Result<List<GenreListNameResponse>>
				{
					Success = true,
					Data = _mapper.Map<List<GenreListNameResponse>>(movielist),
				};
			}
			catch (Exception ex)
			{
				return new Result<List<GenreListNameResponse>> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<GenreReturnMovieListResponse>> GetGenreById(string? id)
		{
			try
			{
				var genre = await _genreRepository.GetGenreById(id);

				if (genre == null)
				{
					return new Result<GenreReturnMovieListResponse>
					{
						Success = false,
						Message = "Not found",
						Data = null
					};
				}

				var per = new GenreReturnMovieListResponse
				{
					Id = genre.Id,
					Description = genre.Description,
					Name = genre.Name,
					movieList = genre.MovieGenres?
                        .Where(mp => mp.Movie != null && mp.Movie.Status == "ACTIVE")
                        .Select(mp => new GenreMovieList
						{
							Id = mp.Movie.Id,
							Title = mp.Movie.Title,
							OriginName = mp.Movie.OriginName,
							Medias = mp.Movie.Media?
								.Where(media => media.Type == "POSTER")
								.Select(media => media.Url)
								.FirstOrDefault()
						})
						.ToList() ?? new List<GenreMovieList>()
				};

				return new Result<GenreReturnMovieListResponse>
				{
					Success = true,
					Data = per
				};
			}
			catch (Exception ex)
			{
				return new Result<GenreReturnMovieListResponse>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public async Task<Result<Genre>> UpdateGenre(string? id, GenreUpdateRequest request)
		{
			try
			{

				var genre = await _genreRepository.GetGenreById(id);
				if (genre == null)
				{
					return new Result<Genre>
					{
						Success = false,
						Message = "Not found",
						Data = genre,
					};
				}
				genre.Name = request.Name;
				genre.Description = request.Description;

				await _genreRepository.Update(genre);
				return new Result<Genre>
				{
					Success = true,
					Data = genre,
				};
			}
			catch (Exception ex)
			{
				return new Result<Genre> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<Genre>> DeleteGenre(string? id)
		{
			try
			{
				var genre = await _genreRepository.GetGenreById(id);
				if (genre == null)
				{
					return new Result<Genre>
					{
						Success = false,
						Message = "Not found",
						Data = genre,
					};
				}
				if (await _genreRepository.DeleteGenreAsync(id) == false)
				{

					return new Result<Genre>
					{
						Success = true,
						Message = "Delete  success"
					};
				}
				else
				{

					return new Result<Genre>
					{
						Success = false,
						Message = "Delete fail"
					};
				}
			}
			catch (Exception ex)
			{
				return new Result<Genre> { Success = false, Message = ex.Message };
			}
		}

	}
}
