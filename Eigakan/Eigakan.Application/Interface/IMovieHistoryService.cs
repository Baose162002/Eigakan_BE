using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.MovieHistory;
using Eigakan.Domain.Response.MovieHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IMovieHistoryService
    {
		Task<(List<MovieHistoryResponse> movieHistories, int Total)> GetAlMovieHistoryAsync(int page, int pageSize);
		Task<Result<MovieHistory>> CreateMovieHistory(MovieHistoryCreateRequest movieHistoryCreateRequest);
	}
}
