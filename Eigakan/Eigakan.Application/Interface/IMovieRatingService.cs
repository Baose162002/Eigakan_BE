using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.MovieRating;


namespace Eigakan.Application.Interface
{
    public interface IMovieRatingService 
    {
        Task<Result<MovieRating>> GetMovieRatingByLogin(string movieId);
		Task<Result<MovieRating>> Rating(MovieRatingCreateRequest request);
        Task<Result<MovieRating>> Update(string? id, MovieRatingUpdateRequest request);
        Task<Result<MovieRating>> Delete(string? id);


    }
}
