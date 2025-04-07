using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MoviesService : IMoviesService
{
    private readonly IValidator<Movie> _movieValidator;
    private readonly IMoviesRepository _repository;

    public MoviesService(IValidator<Movie> movieValidator, IMoviesRepository moviesService)
    {
        ArgumentNullException.ThrowIfNull(movieValidator, nameof(movieValidator));
        ArgumentNullException.ThrowIfNull(moviesService, nameof(moviesService));
        
        _movieValidator = movieValidator;
        _repository = moviesService;
    }

    public async Task<IEnumerable<Movie>> GetAsync() => 
        await _repository.GetAsync();

    public async Task<Movie?> GetByIdAsync(Guid id) => 
        await _repository.GetByIdAsync(id);

    public async Task<Movie?> GetBySlugAsync(string slug) =>
        await _repository.GetBySlugAsync(slug);

    public async Task<bool> CreateAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        
        return await _repository.CreateAsync(movie);
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        await _movieValidator.ValidateAndThrowAsync(movie);
        
        bool movieExists = await _repository.ExistsAsync(movie.Id);

        if (!movieExists)
            return null;

        await _repository.UpdateAsync(movie);
        
        return movie;
    }

    public async Task<bool> DeleteAsync(Guid id) => 
        await _repository.DeleteAsync(id);
}