using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.MovieHistory;
using Eigakan.Domain.Response.Media;
using Eigakan.Domain.Response.MovieHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IMovieCountService 
	{
		Task<Result<MovieCount>> GetMovieCountByMovieId(string? movieId);
		Task<Result<MovieCount>> IncreaseMovieCount(MovieHistoryCreateRequest movieCount);
		Task<object> GetMovieViewStatistics(string movieId);
	}
}
