using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IMovieHistoryRepository : IGenericRepository<MovieHistory>
	{
		Task<List<MovieHistory>> GetAllMovieHistoryByLogin(int page, int pageSize, string userId);
		Task<int> CountAllMovieHistoryAsync(string userId);
		Task<MovieHistory> GetMovieHistoryByUserMovie(string movieId, string userId);
	}
}
