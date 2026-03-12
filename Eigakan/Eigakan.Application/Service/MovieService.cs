using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Media;
using Eigakan.Domain.Request.Movie;
using Eigakan.Domain.Response;
using Eigakan.Domain.Response.ContractResponse;
using Eigakan.Domain.Response.Movie;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Eigakan.Application.Service
{
	public class MovieService : IMovieService
	{

		private readonly IMoviesRepository _moviesRepository;
		private readonly IMapper _mapper;
		private readonly IGenreRepository _genreRepository;
		private readonly IPersonRepository _personRepository;
		private readonly IMoviePersonRepository _moviePersonRepository;
		private readonly IMovieGenreRepository _movieGenreRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICommentRepository _commentRepository;
		private readonly IMovieRatingRepository _movieRatingRepository;
		private readonly IMediaRepository _mediaRepository;
		private readonly Logger _logger;
		private readonly AmazonS3Client _s3Client;
		private readonly string _bucketName;
		public MovieService(Logger logger,
			IMoviesRepository moviesRepository, IGenreRepository genreRepository,
							IPersonRepository personRepository, IMapper mapper, IMovieGenreRepository movieGenreRepository,
							IMoviePersonRepository moviePersonRepository, IHttpContextAccessor httpContextAccessor, 
							ICommentRepository commentRepository, IMovieRatingRepository movieRatingRepository,
							IMediaRepository mediaRepository , IConfiguration configuration
			)
		{
			_commentRepository = commentRepository;
			_moviesRepository = moviesRepository;
			_personRepository = personRepository;
			_mapper = mapper;
			_genreRepository = genreRepository;
			_moviePersonRepository = moviePersonRepository;
			_movieGenreRepository = movieGenreRepository;
			_httpContextAccessor = httpContextAccessor;
			_movieRatingRepository = movieRatingRepository;
			_mediaRepository = mediaRepository;
			//_logger = logger;
			_bucketName = configuration["AWS:BucketName"];
			var accessKey = configuration["AWS:AccessKey"];
			var secretKey = configuration["AWS:SecretKey"];
			var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
			_s3Client = new AmazonS3Client(accessKey, secretKey, region);
		}

		public async Task <(List<MovieGetListResponse> movies, int Total)> GetListAllMovie(int pageNumber, int pageSize,
			string? genreFilter = null, string? nameFilter = null, string? statusFilter = null)
		{
				var query = await _moviesRepository.GetListAllMovie();
				int totalCount = query.Count();

				if (!string.IsNullOrEmpty(nameFilter))
				{
					query = query.Where(movie =>
						(!string.IsNullOrEmpty(movie.Title) && movie.Title.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) ||
						(!string.IsNullOrEmpty(movie.OriginName) && movie.OriginName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(genreFilter))
				{
					query = query.Where(movie =>
						movie.MovieGenres.Any(mg => mg.Genre.Name.Contains(genreFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(statusFilter))
				{
					query = query.Where(movie => movie.Status != null && movie.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
				}

				var movieList1 = query
								.Skip((pageNumber - 1) * pageSize)
								.Take(pageSize)
								.Select(movie => new MovieGetListResponse
								{
									Id = movie.Id,
									Title = movie.Title,
									Description = movie.Description,
									OriginName = movie.OriginName,
									Status = movie.Status,
									UserId = movie.UserId,
									Director = movie.Director,
									Duration = movie.Duration,
									Nation = movie.Nation,
									Rating = movie.Rating,
									IsContract = movie.IsContract,
									ReleaseYear = movie.ReleaseYear,
									FileUrl = movie.FileUrl,
									Script = movie.Script,
									GenreNames = movie.MovieGenres.Any()
												? string.Join(", ", movie.MovieGenres.Select(mg => mg.Genre.Name ?? "Unknown"))
												: "No genres available",
									UserRating = movie.MovieRatings.Any()
												? movie.MovieRatings.Average(r => r.Stars)
												: 0,
									Person = movie.MoviePersons.Select(mp => mp.Person).ToList(),
									Medias = movie.Media
											 .Where(z => z.Type == "POSTER")
											 .Take(1)
											 .ToList()
								}).ToList();

				return (_mapper.Map<List<MovieGetListResponse>>(movieList1), totalCount);
			}


		public async Task <(List<MovieGetListResponse> movies, int Total)> GetListMovieActive(int pageNumber, int pageSize,
			string? genreFilter = null, string? nameFilter = null, string? statusFilter = null)
		{
			
			var query = await _moviesRepository.GetListMovieActive();
				int totalCount = query.Count();

				if (!string.IsNullOrEmpty(nameFilter))
				{
					query = query.Where(movie =>
						(!string.IsNullOrEmpty(movie.Title) && movie.Title.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) ||
						(!string.IsNullOrEmpty(movie.OriginName) && movie.OriginName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(genreFilter))
				{
					query = query.Where(movie =>
						movie.MovieGenres.Any(mg => mg.Genre.Name.Contains(genreFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(statusFilter))
				{
					query = query.Where(movie => movie.Status != null && movie.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
				}

				var movieList1 = query
								.Skip((pageNumber - 1) * pageSize)
								.Take(pageSize)
								.Select(movie => new MovieGetListResponse
								{
									Id = movie.Id,
									Title = movie.Title,
									Description = movie.Description,
									OriginName = movie.OriginName,
									Status = movie.Status,
									UserId = movie.UserId,
									Director = movie.Director,
									Duration = movie.Duration,
									Nation = movie.Nation,
									Rating = movie.Rating,
									IsContract = movie.IsContract,
									ReleaseYear = movie.ReleaseYear,
									Script = movie.Script,
									GenreNames = movie.MovieGenres.Any()
												? string.Join(", ", movie.MovieGenres.Select(mg => mg.Genre.Name ?? "Unknown"))
												: "No genres available",
									UserRating = movie.MovieRatings.Any()
												? movie.MovieRatings.Average(r => r.Stars)
												: 0,
									Person = movie.MoviePersons.Select(mp => mp.Person).ToList(),
									Medias = movie.Media
										.Where(z => z.Type == "POSTER" || z.Type == "BANNER")
										.OrderBy(z => z.Type == "POSTER" ? 0 : 1)
										.ToList()

								}).ToList();

				return (_mapper.Map<List<MovieGetListResponse>>(movieList1), totalCount);
			}
			

		public async Task <(List<MovieGetListResponse> movies, int Total, int ActiveMovie )> GetListAllMovieByLogin(int pageNumber, int pageSize,
			string? genreFilter = null, string? nameFilter = null, string? statusFilter = null)
		{

				var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
				var query = await _moviesRepository.GetListAllMovieByLogin(UserId.Value);
				int totalCount = query.Count();

				if (!string.IsNullOrEmpty(nameFilter))
				{
					query = query.Where(movie =>
						(!string.IsNullOrEmpty(movie.Title) && movie.Title.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) ||
						(!string.IsNullOrEmpty(movie.OriginName) && movie.OriginName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(genreFilter))
				{
					query = query.Where(movie =>
						movie.MovieGenres.Any(mg => mg.Genre.Name.Contains(genreFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(statusFilter))
				{
					query = query.Where(movie => movie.Status != null && movie.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
				}

				var movieList1 = query
								.Skip((pageNumber - 1) * pageSize)
								.Take(pageSize)
								.Select(movie => new MovieGetListResponse
								{
									Id = movie.Id,
									Title = movie.Title,
									Description = movie.Description,
									OriginName = movie.OriginName,
									Status = movie.Status,
									UserId = movie.UserId,
									Director = movie.Director,
									Duration = movie.Duration,
									Nation = movie.Nation,
									Rating = movie.Rating,
									ReleaseYear = movie.ReleaseYear,
									Script = movie.Script,
									IsContract = movie.IsContract,
									GenreNames = movie.MovieGenres.Any()
												? string.Join(", ", movie.MovieGenres.Select(mg => mg.Genre.Name ?? "Unknown"))
												: "No genres available",
									UserRating = movie.MovieRatings.Any()
												? movie.MovieRatings.Average(r => r.Stars)
												: 0,
									Person = movie.MoviePersons.Select(mp => mp.Person).ToList(),
									Medias = movie.Media
											 .Where(z => z.Type == "POSTER")
											 .Take(1)
											 .ToList()
								}).ToList();

				var activeMovie = query.Where(x => x.Status == MovieStatusEnum.ACTIVE.ToString()).Count();

			return (_mapper.Map<List<MovieGetListResponse>>(movieList1), totalCount, activeMovie);
		}

		public async Task<(List<MovieGetListResponse> movies, int Total)> GetListAllMovieByUserId(string userId, int pageNumber, int pageSize,
			string? genreFilter = null, string? nameFilter = null, string? statusFilter = null)
		{

				var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
				var query = await _moviesRepository.GetListAllMovieByLogin(userId);
				int totalCount = query.Count();
				
				if (!string.IsNullOrEmpty(nameFilter))
				{
					query = query.Where(movie =>
						(!string.IsNullOrEmpty(movie.Title) && movie.Title.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) ||
						(!string.IsNullOrEmpty(movie.OriginName) && movie.OriginName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(genreFilter))
				{
					query = query.Where(movie =>
						movie.MovieGenres.Any(mg => mg.Genre.Name.Contains(genreFilter, StringComparison.OrdinalIgnoreCase))
					).ToList();
				}

				if (!string.IsNullOrEmpty(statusFilter))
				{
					query = query.Where(movie => movie.Status != null && movie.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
				}

				var movieList1 = query
								.Skip((pageNumber - 1) * pageSize)
								.Take(pageSize)
								.Select(movie => new MovieGetListResponse
								{
									Id = movie.Id,
									Title = movie.Title,
									Description = movie.Description,
									OriginName = movie.OriginName,
									Status = movie.Status,
									UserId = movie.UserId,
									Director = movie.Director,
									Duration = movie.Duration,
									Nation = movie.Nation,
									Rating = movie.Rating,
									ReleaseYear = movie.ReleaseYear,
									Script = movie.Script,
									GenreNames = movie.MovieGenres.Any()
												? string.Join(", ", movie.MovieGenres.Select(mg => mg.Genre.Name ?? "Unknown"))
												: "No genres available",
									UserRating = movie.MovieRatings.Any()
												? movie.MovieRatings.Average(r => r.Stars)
												: 0,
									Person = movie.MoviePersons.Select(mp => mp.Person).ToList(),
									Medias = movie.Media
											 .Where(z => z.Type == "POSTER")
											 .Take(1)
											 .ToList()
								}).ToList();

			return (_mapper.Map<List<MovieGetListResponse>>(movieList1), totalCount);
		}

		public async Task<Result<MovieGetListResponse>> GetMovieById(string id)
		{
			try
			{
				var movie = await _moviesRepository.GetMovieById(id);
				if (movie == null)
				{
					return new Result<MovieGetListResponse>
					{
						Success = false,
						Message = "Not found",
					};
				}
                var clone = _mapper.Map<MovieGetListResponse>(movie);
                clone.GenreNames = movie.MovieGenres != null && movie.MovieGenres.Any()
                     ? string.Join(", ", movie.MovieGenres
                    .Where(mg => mg.Genre != null) // Kiểm tra null cho Genre
                    .Select(mg => mg.Genre.Name ?? "Unknown")) // Kiểm tra null cho Name
                     : "No genres available";
                clone.UserRating = await _movieRatingRepository.GetAverageRating(clone.Id);
				clone.Person = movie.MoviePersons.Select(mp => mp.Person).ToList();
				clone.contracts = movie.contracts;

				return new Result<MovieGetListResponse>
				{
					Success = true,
					Data = clone,
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieService));
				return new Result<MovieGetListResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<MovieGetById>> GetByMovieIdClear(string id)
		{
			try
			{
				var movie = await _moviesRepository.GetMovieById(id);
				if (movie == null)
				{
					return new Result<MovieGetById>
					{
						Success = false,
						Message = "Not found",
					};
				}
				var clone = _mapper.Map<MovieGetById>(movie);
				clone.GenreNames = movie.MovieGenres != null && movie.MovieGenres.Any()
					 ? string.Join(", ", movie.MovieGenres
					.Where(mg => mg.Genre != null) // Kiểm tra null cho Genre
					.Select(mg => mg.Genre.Name ?? "Unknown")) 
					 : "No genres available";
				clone.Comments = await _commentRepository.GetListMovieId(clone.Id);
				clone.UserRating = await _movieRatingRepository.GetAverageRating(clone.Id);
				clone.Person = movie.MoviePersons.Select(mp => mp.Person).ToList();
				clone.contracts = _mapper.Map<ICollection<ContractGetName>>(movie.contracts);

				return new Result<MovieGetById>
				{
					Success = true,
					Data = clone,
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieService));
				return new Result<MovieGetById> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<MovieGetListResponse>> CreateMovie(CreateMovieRequest movieRequest)
		{
			try
			{
				var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);

				var newMovie = new Movie
				{
					Id = Guid.NewGuid().ToString(),
					Title = movieRequest.Title,
					Description = movieRequest.Description,
					Director = movieRequest.Director,
					Duration = movieRequest.Duration,
					Nation = movieRequest.Nation,
					OriginName = movieRequest.OriginName,
					IsContract = movieRequest.IsContract,
					ReleaseYear = movieRequest.ReleaseYear,
					Script = movieRequest.Script,
					FileUrl = movieRequest.FileUrl,
					Status = MovieStatusEnum.WAITING_FOR_REVIEWING.ToString(),
					CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
					UserId = UserId.Value,
					Media = movieRequest.Medias?.Select(m => new Media
					{
						Id = Guid.NewGuid().ToString(),
						Name = m.Name,
						Url = m.Url,
						Type = m.Type,
						CreateDate = DateTime.Now,

					}).ToList() ?? new List<Media>()
				};

				var genreIds = movieRequest.Genres ?? new List<string>();
				var personIds = movieRequest.Persons ?? new List<string>();

				// 🔹 Lấy danh sách Genres và Persons từ database
				var genres = genreIds.Any() ? await _genreRepository.GetListGenreById(genreIds) : new List<string>();
				var actors = personIds.Any() ? await _personRepository.GetListPersonById(personIds) : new List<string>();

				// 🔹 Tạo danh sách MoviePersons nếu có diễn viên
				if (actors.Any())
				{
					newMovie.MoviePersons = actors.Select(personId => new MoviePerson
					{
						MovieId = newMovie.Id,
						PersonId = personId,
					}).ToList();
				}

				// 🔹 Tạo danh sách MovieGenres nếu có thể loại
				if (genres.Any())
				{
					newMovie.MovieGenres = genres.Select(genre => new MovieGenre
					{
						MovieId = newMovie.Id,
						GenreId = genre.ToString(),
					}).ToList();
				}

				// Thêm movie vào database
				await _moviesRepository.Insert(newMovie);
				return await GetMovieById(newMovie.Id);


			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieService));
				return new Result<MovieGetListResponse>
				{
					Success = false,
					Message = "ERROR!"
				};
			}
		}

		public async Task<Result<MovieGetListResponse>> UpdateMovie(string movieId, UpdateMovieRequest movieRequest)
		{
			var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
			var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

			// 🔹 Tìm phim trong database
			var existingMovie = await _moviesRepository.GetMovieById(movieId);
			if (existingMovie == null)
			{
				return new Result<MovieGetListResponse>
				{
					Success = false,
					Message = "NOT FOUND!"
				};
			}

			if (roleClaim.Value != "ADMIN" && existingMovie.UserId != UserId.Value)
			{
				return new Result<MovieGetListResponse>
				{
					Success = false,
					Message = "YOU DON'T HAVE AUTHORIZATION"
				};
			}

			
			existingMovie.Title = movieRequest.Title ?? existingMovie.Title;
			existingMovie.Description = movieRequest.Description ?? existingMovie.Description;
			existingMovie.Director = movieRequest.Director ?? existingMovie.Director;
			existingMovie.Duration = movieRequest.Duration ?? existingMovie.Duration;
			existingMovie.Nation = movieRequest.Nation ?? existingMovie.Nation;
			existingMovie.OriginName = movieRequest.OriginName ?? existingMovie.OriginName;
			existingMovie.ReleaseYear = movieRequest.ReleaseYear ?? existingMovie.ReleaseYear;
			existingMovie.Script = movieRequest.Script ?? existingMovie.Script;
			existingMovie.IsContract = movieRequest.IsContract ?? existingMovie.IsContract;
			existingMovie.UpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
			existingMovie.Status = movieRequest.Status ?? MovieStatusEnum.WAITING_FOR_REVIEWING.ToString();
			existingMovie.ReasonForRejection = null;
			existingMovie.FileUrl = movieRequest.FileUrl ?? existingMovie.FileUrl;

			// Xử lý Genres (thể loại)
			var newGenres = movieRequest.Genres ?? new List<string>(); 
			var currentGenreIds = existingMovie.MovieGenres.Select(mg => mg.GenreId).ToList();

			// Thêm thể loại mới
			var genresToAdd = newGenres.Except(currentGenreIds).ToList();
			var genreEntitiesToAdd = await _genreRepository.GetListGenreById(genresToAdd);
			foreach (var genre in genreEntitiesToAdd)
			{
				var gen = new MovieGenre
				{
					MovieId = existingMovie.Id,
					GenreId = genre.ToString()
				};

				await _movieGenreRepository.Insert(gen);
			}


			// Xóa thể loại bị loại bỏ
			var genresToRemove = existingMovie.MovieGenres
			 .Where(mg => !newGenres.Contains(mg.GenreId))
			  .ToList(); 

			foreach (var item in genresToRemove)
			{
				existingMovie.MovieGenres.Remove(item);
			}

			await _movieGenreRepository.DeleteRange(genresToRemove);


			// 🔹 Xử lý Persons (diễn viên)
			var newPersons = movieRequest.Persons ?? new List<string>(); // Danh sách diễn viên mới
			var currentPersonIds = existingMovie.MoviePersons.Select(mp => mp.PersonId).ToList();

			// Thêm diễn viên mới
			var personsToAdd = newPersons.Except(currentPersonIds).ToList();
			var personEntitiesToAdd = await _personRepository.GetListPersonById(personsToAdd);
			foreach (var person in personEntitiesToAdd)
			{
				var movieperson = new MoviePerson
				{
					MovieId = existingMovie.Id,
					PersonId = person
				};
				
				await _moviePersonRepository.Insert(movieperson);
			}


			var itemsToRemoveList = existingMovie.MoviePersons
			 .Where(mp => !newPersons.Contains(mp.PersonId))
			.ToList();

			
			await _moviePersonRepository.DeleteRange(itemsToRemoveList);

			//media

			var mediasToRemove = existingMovie.Media.ToList();

			await _mediaRepository.DeleteRange(mediasToRemove);


			var newMedias = movieRequest.Medias ?? new List<MediaMovieCreateRequest>();

			foreach (var media in newMedias)
			{
				var medi = new Media
				{
					Id = Guid.NewGuid().ToString(),
					MovieId = existingMovie.Id,
					CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
					Name = media.Name,
					Type = media.Type,
					Url = media.Url,
				};

				await _mediaRepository.Insert(medi);
			}


			try
			{
				// 🔹 Lưu thay đổi vào database
				await _moviesRepository.Update(existingMovie);
				return new Result<MovieGetListResponse>
				{
					Success = true,
					Message = "Update movie successfully",
				};

			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieService));
				return new Result<MovieGetListResponse>
				{
					Success = false,
					Message = "Error!"
				};
			}
		}

		public async Task<Result<MovieGetListResponse>> ArchivedMovie(string? id)
		{
			try
			{
				var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

				var movie = await _moviesRepository.GetMovieById(id);
				if (movie == null)
				{
					return new Result<MovieGetListResponse>
					{
						Success = false,
						Message = "Not found",

					};
				}

				if (roleClaim?.Value != "ADMIN")
				{
					return new Result<MovieGetListResponse>
					{
						Success = false,
						Message = "YOU DON'T HAVE AUTHORIZATION"
					};
				}
				movie.Status = MovieStatusEnum.ARCHIVED.ToString();

				await _moviesRepository.Update(movie);
				return new Result<MovieGetListResponse>
				{
					Success = true,
					Message = "Archived movie successfully"
				};


			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(MovieService));
				return new Result<MovieGetListResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<Movie>> AcceptedMovie(AcceptedMovieRequest acceptedMovieRequest)
		{
			try
			{
				var existingMovie = await _moviesRepository.GetMovieById(acceptedMovieRequest.Id);
				if (existingMovie == null)
					return new Result<Movie> { Success = false, Message = "Id does not exist" };

				var newStatus = existingMovie.Status switch
				{
					var status when status == MovieStatusEnum.WAITING_FOR_REVIEWING.ToString()
						=> MovieStatusEnum.ACCEPTED_NEGOTIATING.ToString(),

					var status when status == MovieStatusEnum.ACCEPTED_NEGOTIATING.ToString()
						=> MovieStatusEnum.ACTIVE.ToString(),
					
					var status when status == MovieStatusEnum.ARCHIVED.ToString()
					=> MovieStatusEnum.ACTIVE.ToString(),
					_ => null
				};

				if (newStatus == null)
					return new Result<Movie> { Success = false, Message = "Can not update this Movie" };

				existingMovie.Status = newStatus;
				await _moviesRepository.Update(existingMovie);

				return new Result<Movie> { Success = true, Message = "Update status successful" };
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(Movie));
				return new Result<Movie> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<Movie>> AcceptedMovieNotContract(AcceptedMovieRequest acceptedMovieRequest)
		{
			try
			{
				var existingMovie = await _moviesRepository.GetMovieById(acceptedMovieRequest.Id);
				if (existingMovie == null)
					return new Result<Movie> { Success = false, Message = "Id does not exist" };

				if (string.IsNullOrEmpty(existingMovie.FileUrl))
					return new Result<Movie> { Success = false, Message = "File copy-right not found, please upload again!" };


				var newStatus = existingMovie.Status switch
				{
					var status when status == MovieStatusEnum.WAITING_FOR_REVIEWING.ToString()
						=> MovieStatusEnum.ACTIVE.ToString(),

					_ => null
				};

				if (newStatus == null)
					return new Result<Movie> { Success = false, Message = "Can not update this Movie" };

				existingMovie.Status = newStatus;
				await _moviesRepository.Update(existingMovie);

				var updateUrlMovie = await MoveFileToMovieFolderAsync(existingMovie.FileUrl, acceptedMovieRequest.Id);
				existingMovie.FileUrl = updateUrlMovie;

				await _moviesRepository.Update(existingMovie);

				return new Result<Movie> { Success = true, Message = "Update status successful" };
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(Movie));
				return new Result<Movie> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<Movie>> RejectedMovie(RejectedMovieRequest rejectedMovieRequest)
		{
			try
			{
				var existingMovie = await _moviesRepository.GetMovieById(rejectedMovieRequest.Id);

				if (existingMovie == null)
					return new Result<Movie> { Success = false, Message = "Id does not exist" };

				if (existingMovie.Status == MovieStatusEnum.WAITING_FOR_REVIEWING.ToString())
				{
					// Cập nhật trạng thái của Movie

					existingMovie.Status = MovieStatusEnum.REJECTED.ToString();
					existingMovie.ReasonForRejection = rejectedMovieRequest.ReasonForRejection;

					await _moviesRepository.Update(existingMovie);

					return new Result<Movie>
					{
						Success = true,
						Message = "Update status successfull"
					};
				}
				return new Result<Movie>
				{
					Success = false,
					Message = "Can not update this Movie"
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(Movie));
				return new Result<Movie> { Success = false, Message = ex.Message };
			}
		}

		//di chuyển folder tạm sang chính với uid user
		private async Task<string> MoveFileToMovieFolderAsync(string tempFileUrl, string movieId)
		{
			var tempFileMatch = Regex.Match(tempFileUrl, @".*/temp-uploads/(?<id>[a-f0-9-]+)/(?<filename>.+)");
			if (tempFileMatch.Success)
			{
				var fileId = tempFileMatch.Groups["id"].Value;
				var fileName = tempFileMatch.Groups["filename"].Value;

				var sourceKey = $"temp-uploads/{fileId}/{fileName}";
				var destinationKey = $"movie-uploads/{movieId}/{fileName}";

				// Copy file từ temp-uploads vào user-uploads/{userId}/
				var copyRequest = new CopyObjectRequest
				{
					SourceBucket = "file-eigakan",
					DestinationBucket = "file-eigakan",
					SourceKey = sourceKey,
					DestinationKey = destinationKey
				};

				await _s3Client.CopyObjectAsync(copyRequest);

				// Xóa file trong temp-uploads
				//await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
				//{
				//	BucketName = "file-eigakan",
				//	Key = sourceKey
				//});


				return $"https://file-eigakan.s3.ap-southeast-2.amazonaws.com/{destinationKey}";
			}

			throw new ArgumentException("Invalid temp file URL", nameof(tempFileUrl));
		}

	}
}
