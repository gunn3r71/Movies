using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public interface IMoviesRepository
    {
        Task<IEnumerable<Movie>> GetAsync(CancellationToken cancellationToken = default);
        Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}