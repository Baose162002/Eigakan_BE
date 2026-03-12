using DocumentFormat.OpenXml.Spreadsheet;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MovieRepositories
{
	public class MovieRepository : GenericBase<Domain.Models.Movie>, IMoviesRepository
	{
		private readonly EigakanDbContext _context;

		public MovieRepository(EigakanDbContext context)
		{
			_context = context;
		}

		public async Task<List<Domain.Models.Movie>> GetListMovieActive()
		{
			var movieList = await _context.Movies
				.Where(m => m.Status == MovieStatusEnum.ACTIVE.ToString())
		.Include(m => m.MovieRatings)
		.Include(m => m.MovieGenres)
		.ThenInclude(mg => mg.Genre)
		  .Include(m => m.Media).Include(m => m.MoviePersons)
						.ThenInclude(mp => mp.Person)
		.OrderByDescending(m => m.CreatedDate)
		.ToListAsync();
			return movieList;
		}

		public async Task<List<Domain.Models.Movie>> GetListAllMovie()
		{
			var movieList = await _context.Movies
		.Include(m => m.MovieRatings)
		.Include(m => m.MovieGenres)
		.ThenInclude(mg => mg.Genre)
		  .Include(m => m.Media).Include(m => m.MoviePersons)
						.ThenInclude(mp => mp.Person)
		.OrderByDescending(m => m.CreatedDate)
		.ToListAsync();
			return movieList;
		}

		public async Task<List<Domain.Models.Movie>> GetListAllMovieByLogin(string userID)
		{
			if (string.IsNullOrWhiteSpace(userID))
			{
				return new List<Domain.Models.Movie>(); 
			}

			return await _context.Movies
				.AsNoTracking()
				.Where(m => m.UserId == userID)
				.Include(m => m.MovieRatings)
				.Include(m => m.MovieGenres)
					.ThenInclude(mg => mg.Genre)
				.Include(m => m.Media)
				.Include(m => m.MoviePersons)
					.ThenInclude(mp => mp.Person)
				.OrderByDescending(m => m.CreatedDate)
				.ToListAsync();
		}

		public async Task<Movie> GetMovieById(string id)
		{
			try
			{
				var movie = await _context.Movies
					.Include(m => m.MovieGenres)
						.ThenInclude(mg => mg.Genre)
					.Include(m => m.Media)
					.Include(m => m.Comments)
					.Include(m => m.MoviePersons)
						.ThenInclude(mp => mp.Person)
					.Include(m => m.User)
					.Include(m => m.MovieCounts)
					.Include(m => m.contracts)
					.FirstOrDefaultAsync(m => m.Id == id);

				return movie;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<bool> DeleteMovieAsync(string? Id)
		{
			var moviePerson = _context.MoviePersons.Where(mg => mg.MovieId == Id);
			_context.MoviePersons.RemoveRange(moviePerson);
			var movieGenre = _context.MovieGenres.Where(mg => mg.MovieId == Id);
			_context.MovieGenres.RemoveRange(movieGenre);
			var movie = await _context.Movies.FindAsync(Id);
			if (movie != null)
			{
				_context.Movies.Remove(movie);
				await _context.SaveChangesAsync();
				return true;
			}

			return false;
		}

		public async Task<List<Movie>> GetAllMovieNotContractByLogin(string userId)
		{
			return (await Get(
				includeProperties: "MovieCounts",
				filter: q => ( q.UserId == userId && q.IsContract == false)
			))
			.ToList();
		}
   
		public async Task UpdateMovieStatusAsync(string movieId, string newStatus)
        {
            var movie = await GetMovieById(movieId);
            if (movie != null)
            {
                movie.Status = newStatus;
                await Update(movie);
            }
        }
   
		public async Task<List<Movie>> GetListMovieByDate(DateOnly startDate, DateOnly endDate)
		{
			return (await Get(
				includeProperties: "MovieCounts",
				filter: q => q.MovieCounts.Any(mc => mc.ViewDate >= startDate && mc.ViewDate <= endDate)
			)).ToList();
		}

		public async Task<int> CountAllMovieAsync()
		{
			return await CountAsync();
		}

		public async Task<int> CountAllMovieActiveAsync()
		{
			return await CountAsync(p => p.Status == "ACTIVE");
		}

	}
}
