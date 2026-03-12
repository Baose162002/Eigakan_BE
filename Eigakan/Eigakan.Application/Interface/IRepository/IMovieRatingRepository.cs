using Eigakan.Domain.Models;


namespace Eigakan.Application.Interface.IRepository
{
    public interface IMovieRatingRepository : IGenericRepository<MovieRating>
    {
        
        Task<List<MovieRating>> GetList();
        Task<List<MovieRating>> GetListMovieRatingByMovieId(string? movieId);
        Task<MovieRating> GetMovieRatingById(string id);
		Task<MovieRating> GetMovieRatingByLogin(string id, string movieId);
		Task<bool> DeleteMovieRatingAsync(string? Id);
        Task<MovieRating> GetMovieRatingByUserId(string id, string movieid);
        Task<double?> GetAverageRating(string? movieId);
    }
}
