using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMoviesService
{
    Task<IEnumerable<Movie>> GetAsync();
    Task<Movie?> GetByIdAsync(Guid id);
    Task<Movie?> GetBySlugAsync(string slug);
    Task<bool> CreateAsync(Movie movie);
    Task<Movie?> UpdateAsync(Movie movie);
    Task<bool> DeleteAsync(Guid id);
}