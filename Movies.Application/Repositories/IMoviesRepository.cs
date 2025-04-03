using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public interface IMoviesRepository
    {
        Task<IEnumerable<Movie>> GetMoviesAsync();
        Task<Movie?> GetMovieAsync(Guid id);
        Task<Movie?> GetMovieBySlugAsync(string slug);
        Task<bool> CreateMovieAsync(Movie movie);
        Task<bool> UpdateMovieAsync(Movie movie);
        Task<bool> DeleteMovieAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}