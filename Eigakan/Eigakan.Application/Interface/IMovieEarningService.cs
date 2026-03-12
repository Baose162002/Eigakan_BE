using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.MovieEarning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IMovieEarningService
	{
		Task<MovieEarningDashboardResponse> GetAllMovieEarningAsync(int page, int pageSize);
		Task<Result<MovieEarning>> GetMovieEarningById(string id);
		Task<(List<MovieEarningResponse> movieEarningMovieId, int total, decimal totalEarning)> GetAllMovieEarningByMovieId(int page, int pageSize, DateOnly? startDate, DateOnly? endDate, string movieId);
	}
}
