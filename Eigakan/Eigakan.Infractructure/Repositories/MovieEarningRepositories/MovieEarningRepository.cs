using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MovieEarningRepositories
{
	public class MovieEarningRepository : GenericBase<MovieEarning>, IMovieEarningRepository
	{
		public async Task<List<MovieEarning>> GetAllMovieEarningAsync(int page, int pageSize)
		{
			return (await Get(
				includeProperties: "Movie",
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				pageIndex: page,
				pageSize: pageSize
			))
			.ToList();
		}
		
		public async Task<List<MovieEarning>> GetAllMovieEarningAsyncNoPaging()
		{
			return (await Get(
				includeProperties: "Movie"
			))
			.ToList();
		}

		public async Task<List<MovieEarning>> GetAllMovieEarningAsyncNoPagingByMovieId(string movieId)
		{
			return (await Get(
				includeProperties: "Movie",
				filter: q => q.MovieId == movieId
			))
			.ToList();
		}

		public async Task<List<MovieEarning>> GetAllMovieEarningByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string userId)
		{
			return (await Get(
				includeProperties: "Movie",
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				filter: q =>
					q.UserId == userId &&
					q.Movie.IsContract == false &&	
					(!startDate.HasValue || q.StartWeek == startDate.Value) &&
					(!endDate.HasValue || q.EndWeek == endDate.Value),
				pageIndex: page,
				pageSize: pageSize
			)).ToList();
		}

		public async Task<List<MovieEarning>> GetAllMovieEarningByMovieId(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string movieId)
		{
			return (await Get(
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				filter: q =>
					q.MovieId == movieId &&
					(!startDate.HasValue || q.StartWeek == startDate.Value) &&
					(!endDate.HasValue || q.EndWeek == endDate.Value),
				pageIndex: page,
				pageSize: pageSize
			)).ToList();
		}

		public async Task<int> CountAllMovieEarningAsync()
		{
			return await CountAsync();
		}

		public async Task<int> CountAllMovieEarningByMovieId(string movieId)
		{
			return await CountAsync(q => q.MovieId == movieId);
		}

		public async Task<int> CountAllMovieEarningByUserId(string userId)
		{
			return await CountAsync(q => q.UserId == userId);
		}

		public async Task<MovieEarning> GetMovieEarningById(string id)
		{
			return await GetSingle(u => u.Id.Equals(id));
		}

		public async Task<List<MovieEarning>> GetListMovieEarningByDate(DateOnly startDate, DateOnly endDate)
		{
			return (await Get(
				includeProperties: "Movie",
				filter: q => q.StartWeek == startDate && q.EndWeek == endDate
			)).ToList();
		}
	}
}