using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Genre;
using Eigakan.Domain.Request.Movie;
using Eigakan.Domain.Response.Genre;
using Eigakan.Domain.Response.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IGenreService
    {
        Task<Result<List<GenreListNameResponse>>> GetList();
        Task<Result<Genre>> CreateGenre(CreateGenreRequest movieRequest);
        Task<Result<Genre>> UpdateGenre(string? id, GenreUpdateRequest request);

        Task<Result<GenreReturnMovieListResponse>> GetGenreById(string? id);
        Task<Result<Genre>> DeleteGenre(string? id);
    }
}
