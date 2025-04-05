using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MoviesService : IMoviesService
{
    private readonly IMoviesRepository _repository;

    public MoviesService(IMoviesRepository moviesService)
    {
        ArgumentNullException.ThrowIfNull(moviesService, nameof(moviesService));
        
        _repository = moviesService;
    }

    public async Task<IEnumerable<Movie>> GetAsync() => 
        await _repository.GetAsync();

    public async Task<Movie?> GetByIdAsync(Guid id) => 
        await _repository.GetByIdAsync(id);

    public async Task<Movie?> GetBySlugAsync(string slug) =>
        await _repository.GetBySlugAsync(slug);

    public async Task<bool> CreateAsync(Movie movie) =>
        await _repository.CreateAsync(movie);
    
    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        bool movieExists = await _repository.ExistsAsync(movie.Id);

        if (!movieExists)
            return null;

        await _repository.UpdateAsync(movie);
        
        return movie;
    }

    public async Task<bool> DeleteAsync(Guid id) => 
        await _repository.DeleteAsync(id);
}