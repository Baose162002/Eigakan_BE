using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Movie;
using Eigakan.Domain.Response.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IMovieService
    {
        Task<(List<MovieGetListResponse> movies, int Total)> GetListAllMovie(int pageNumber, int pageSize,string? genreFilter = null, string? nameFilter = null, string? statusFilter = null);
        Task<(List<MovieGetListResponse> movies, int Total)> GetListMovieActive(int pageNumber, int pageSize,string? genreFilter = null, string? nameFilter = null, string? statusFilter = null);
        Task<(List<MovieGetListResponse> movies, int Total, int ActiveMovie)> GetListAllMovieByLogin(int pageNumber, int pageSize,string? genreFilter = null, string? nameFilter = null, string? statusFilter = null);
        Task<(List<MovieGetListResponse> movies, int Total)> GetListAllMovieByUserId(string userId, int pageNumber, int pageSize, string? genreFilter = null, string? nameFilter = null, string? statusFilter = null);
		Task<Result<MovieGetListResponse>> CreateMovie(CreateMovieRequest movieRequest);
        Task<Result<MovieGetById>> GetByMovieIdClear(string id);
        Task<Result<MovieGetListResponse>> GetMovieById(string id);
        Task<Result<MovieGetListResponse>> UpdateMovie(string movieId, UpdateMovieRequest movieRequest);
        Task<Result<Movie>> AcceptedMovie(AcceptedMovieRequest acceptedMovieRequest);
        Task<Result<Movie>> RejectedMovie(RejectedMovieRequest rejectedMovieRequest);
		Task<Result<MovieGetListResponse>> ArchivedMovie(string? id);
        Task<Result<Movie>> AcceptedMovieNotContract(AcceptedMovieRequest acceptedMovieRequest);

	}
}
