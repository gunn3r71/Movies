using Dapper;
using Movies.Application.Database.Factory;

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
                                 MAX(CASE WHEN r.user_id = @UserId THEN r.rating END) AS user_rating
                             FROM ratings r
                             WHERE r.movie_id = @MovieId;
                             """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var result = await connection.QuerySingleAsync(new CommandDefinition(query, new { UserId = userId, MovieId = movieId }, cancellationToken: cancellationToken));

        if (result == null)
            return (null, null);

        return ((float?) result.rating, (byte?) result.user_rating);
    }
}