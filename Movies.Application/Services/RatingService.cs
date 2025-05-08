using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _repository;
    private readonly IMoviesRepository _moviesRepository;

    public RatingService(IRatingRepository repository, IMoviesRepository moviesRepository)
    {
        _repository = repository;
        _moviesRepository = moviesRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, byte rating, CancellationToken cancellationToken = default)
    {
        if (rating is <= 0 or > 5)
            throw new ValidationException([new ValidationFailure("Rating", "Rating must be between 0 and 5")]);
        
        var movieExists = await _moviesRepository.ExistsAsync(movieId, cancellationToken);

        if (!movieExists)
            return false;
        
        return await _repository.RateMovieAsync(movieId, userId, rating, cancellationToken);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteRatingAsync(movieId, userId, cancellationToken);
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetRatingsByUserAsync(userId, cancellationToken);
    }
}