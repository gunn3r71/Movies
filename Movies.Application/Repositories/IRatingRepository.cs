using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IRatingRepository
{
    Task<float?> GetRatingAsync(Guid? movieId, CancellationToken cancellationToken = default);
    Task<(float? Rating, byte? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> RateMovieAsync(Guid movieId, Guid userId, byte rating, CancellationToken cancellationToken = default);
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MovieRating>> GetRatingsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}