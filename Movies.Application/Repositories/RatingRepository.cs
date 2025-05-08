using Dapper;
using Movies.Application.Database.Factory;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RatingRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<float?> GetRatingAsync(Guid? movieId, CancellationToken cancellationToken = default)
    {
        const string query = """
                             SELECT 
                                 round(avg(r.rating), 1) as rating
                             FROM Ratings r
                             WHERE r.movie_id = @MovieId;
                             """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var result = await connection.QuerySingleAsync(new CommandDefinition(query, new { MovieId = movieId }, cancellationToken: cancellationToken));

        if (result.rating == null)
            return null;
        
        return (float) result.rating;
    }

    public async Task<(float? Rating, byte? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        
        const string query = """
                             SELECT
                                 ROUND(AVG(r.rating), 1) AS rating,
                                 MAX(CASE    WHEN r.user_id = @UserId THEN r.rating END) AS user_rating
                             FROM ratings r
                             WHERE r.movie_id = @MovieId;
                             """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var result = await connection.QuerySingleAsync(new CommandDefinition(query, new { UserId = userId, MovieId = movieId }, cancellationToken: cancellationToken));

        return result == null ? (null, null) : ((float?) result.rating, (byte?) result.user_rating);
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, byte rating, CancellationToken cancellationToken = default)
    {
        const string insertCommand = """
                                     INSERT INTO Ratings (movie_id, user_id, rating) VALUES (@MovieId, @UserId, @Rating)
                                        ON CONFLICT (movie_id, user_id) DO UPDATE
                                         SET rating = @Rating;
                                     """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var result = await connection.ExecuteAsync(new CommandDefinition(insertCommand, new { MovieId = movieId, UserId = userId, Rating = rating }, cancellationToken: cancellationToken));
        
        return result > 0; 
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        const string deleteRatingCommand = "DELETE FROM Ratings WHERE movie_id = @MovieId AND user_id = @UserId;";
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var result = await connection.ExecuteAsync(new CommandDefinition(deleteRatingCommand, new { MovieId = movieId, UserId = userId }, cancellationToken: cancellationToken));
        
        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string query = """
                             SELECT r.movie_id, r.rating, m.slug FROM ratings r
                             INNER JOIN movies m 
                                        ON r.movie_id = m.id
                             WHERE r.user_id = @UserId;
                             """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        return await connection.QueryAsync<MovieRating>(new CommandDefinition(query, new { UserId = userId }, cancellationToken: cancellationToken));
    }
}