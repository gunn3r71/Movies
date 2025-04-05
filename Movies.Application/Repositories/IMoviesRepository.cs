using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public interface IMoviesRepository
    {
        Task<IEnumerable<Movie>> GetAsync();
        Task<Movie?> GetByIdAsync(Guid id);
        Task<Movie?> GetBySlugAsync(string slug);
        Task<bool> CreateAsync(Movie movie);
        Task<bool> UpdateAsync(Movie movie);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}