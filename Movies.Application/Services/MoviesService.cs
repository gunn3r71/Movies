using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MoviesService : IMoviesService
{
    private readonly IValidator<Movie> _movieValidator;
    private readonly IMoviesRepository _repository;
    private readonly IRatingRepository _ratingRepository;

    public MoviesService(IValidator<Movie> movieValidator, IMoviesRepository moviesService, IRatingRepository ratingRepository)
    {
        ArgumentNullException.ThrowIfNull(movieValidator, nameof(movieValidator));
        ArgumentNullException.ThrowIfNull(moviesService, nameof(moviesService));
        
        _movieValidator = movieValidator;
        _repository = moviesService;
        _ratingRepository = ratingRepository;
    }

    public async Task<IEnumerable<Movie>> GetAsync(Guid? userId = null, CancellationToken cancellationToken = default) => 
        await _repository.GetAsync(userId, cancellationToken);

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default) => 
        await _repository.GetByIdAsync(id, userId, cancellationToken);

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null, CancellationToken cancellationToken = default) =>
        await _repository.GetBySlugAsync(slug, userId, cancellationToken);

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);
        
        return await _repository.CreateAsync(movie, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);
        
        bool movieExists = await _repository.ExistsAsync(movie.Id, cancellationToken);

        if (!movieExists)
            return null;

        await _repository.UpdateAsync(movie, cancellationToken);

        if (userId.HasValue)
        {
            var (rating, userRating) = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, cancellationToken);
            movie.UserRating = userRating;
            movie.Rating = rating;
        }
        else
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, cancellationToken);

            movie.Rating = rating;
        }
        
        return movie;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) => 
        await _repository.DeleteAsync(id, cancellationToken);
}