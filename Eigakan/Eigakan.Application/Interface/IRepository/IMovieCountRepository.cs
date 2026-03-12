using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
	public interface IMovieCountRepository : IGenericRepository<MovieCount>
	{
		Task<MovieCount> GetMovieCountByMovieId(string movieId);
		Task<MovieCount> CheckCountByMovieDate(string movieId, DateOnly dateTime);
		Task<List<MovieCount>> GetAllMovieCountByMovieId(string movieId);
	}
}
