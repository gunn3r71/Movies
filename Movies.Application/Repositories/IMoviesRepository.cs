using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public interface IMoviesRepository
    {
        Task<IEnumerable<Movie>> GetAsync(Guid? userId = null, CancellationToken cancellationToken = default);
        Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
        Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null, CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}