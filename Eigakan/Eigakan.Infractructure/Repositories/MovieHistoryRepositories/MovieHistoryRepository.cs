using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MovieHistoryRepositories
{
	public class MovieHistoryRepository : GenericBase<MovieHistory>, IMovieHistoryRepository
	{
		public async Task<List<MovieHistory>> GetAllMovieHistoryByLogin(int page, int pageSize, string userId)
		{
			return (await Get(
				filter: c => c.UserId == userId,
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				includeProperties: "Movie,User,Movie.Media",
				pageIndex: page,
				pageSize: pageSize
			))
			.ToList();
		}

		public async Task<int> CountAllMovieHistoryAsync(string userId)
		{
			return await CountAsync(u => u.UserId == userId);
		}

		public async Task<MovieHistory> GetMovieHistoryByUserMovie(string movieId, string userId)
		{
			return (await GetSingle(u => u.UserId == userId && u.MovieId == movieId));
		}

	}
}