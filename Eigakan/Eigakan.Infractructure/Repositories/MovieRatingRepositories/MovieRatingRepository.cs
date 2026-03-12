using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MovieRatingRepositories
{
    public class MovieRatingRepository:GenericBase<MovieRating>,IMovieRatingRepository
    {
        private readonly EigakanDbContext _context;

        public MovieRatingRepository(EigakanDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovieRating>> GetList()
        {
            var ratings = await _context.MovieRating
            .OrderByDescending(c => c.CreateDate)

            .ToListAsync();
            return ratings;
        }
        public async Task<List<MovieRating>> GetListMovieRatingByMovieId(string? movieId)
        {
            var ratings = await _context.MovieRating
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreateDate)

                .ToListAsync();

            return ratings;
        }

        public async Task<MovieRating> GetMovieRatingById(string id)
        {
            return await GetSingle(u => u.Id.Equals(id));
        }

		public async Task<MovieRating> GetMovieRatingByLogin(string id, string movieId)
		{
			return await GetSingle(
				u => u.UserId == id && u.MovieId == movieId,
				includeProperties: "User,Movie"
				);
		}

		public async Task<MovieRating> GetMovieRatingByUserId(string id,string movieid)
        {
            return await _context.MovieRating.AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == id &&  c.MovieId == movieid);
        }
        
        public async Task<bool> DeleteMovieRatingAsync(string? Id)
        {


            var rating = await _context.MovieRating.FindAsync(Id);
            if (rating != null)
            {
                _context.MovieRating.Remove(rating);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<double?> GetAverageRating(string movieId)
        {
            var ratings = await _context.MovieRating
                .Where(r => r.MovieId == movieId && r.Stars.HasValue)
                .Select(r => r.Stars.Value)
                .ToListAsync();

            if (!ratings.Any()) return 0; // Trả về 0 nếu chưa có đánh giá

            return Math.Round(ratings.Average(), 2);
        }
    }
}
