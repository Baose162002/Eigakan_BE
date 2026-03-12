using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IMoviesRepository : IGenericRepository<Movie>
    {
        Task<List<Domain.Models.Movie>> GetListMovieActive();
        Task<List<Domain.Models.Movie>> GetListAllMovie();
		Task<Movie> GetMovieById(string id);
        Task<bool> DeleteMovieAsync(string? id);
        Task<List<Domain.Models.Movie>> GetListAllMovieByLogin(string userID);
        Task<List<Movie>> GetAllMovieNotContractByLogin(string userId);
        Task<List<Movie>> GetListMovieByDate(DateOnly startDate, DateOnly endDate);
        Task UpdateMovieStatusAsync(string movieId, string newStatus);
        Task<int> CountAllMovieAsync();
        Task<int> CountAllMovieActiveAsync();


	}
}
