using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Movie;
using Eigakan.Domain.Response.Genre;
using Eigakan.Domain.Response.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IGenreRepository : IGenericRepository<Genre>
    {
        Task<List<Genre>> GetList();    
        Task<Genre> GetListMovieByGenreId(int id);
      
        Task<int> CheckName(string? names);
        Task<List<string>> GetListGenreById(List<string>? genres);
        Task<Genre> GetGenreById(string? id);

        Task<bool> DeleteGenreAsync(string? Id);

    }
}
