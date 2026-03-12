using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IMovieEarningRepository : IGenericRepository<MovieEarning>
	{
		Task<List<MovieEarning>> GetAllMovieEarningAsync(int page, int pageSize);
		Task<List<MovieEarning>> GetAllMovieEarningAsyncNoPaging();
		Task<List<MovieEarning>> GetAllMovieEarningAsyncNoPagingByMovieId(string movieId);
		Task<int> CountAllMovieEarningAsync();
		Task<int> CountAllMovieEarningByMovieId(string movieId);
		Task<int> CountAllMovieEarningByUserId(string userId);
		Task<MovieEarning> GetMovieEarningById(string id);
		Task<List<MovieEarning>> GetListMovieEarningByDate(DateOnly startDate, DateOnly endDate);
		Task<List<MovieEarning>> GetAllMovieEarningByLogin(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string userId);
		Task<List<MovieEarning>> GetAllMovieEarningByMovieId(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string movieId);
	}
}
