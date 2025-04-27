using Dapper;
using Movies.Application.Database.Factory;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MoviesRepository : IMoviesRepository
{
    private const string InsertGenreCommand = """
                                              INSERT INTO genres (movie_id, name)
                                              VALUES (@MovieId, @Name);
                                              """;
    
    private const string DeleteGenresCommand = "DELETE FROM genres WHERE movie_id = @MovieId;";
    
    private readonly IDbConnectionFactory _connectionFactory;

    public MoviesRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory, nameof(connectionFactory));
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Movie>> GetAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT mvs.*, 
                                  string_agg(g.name, ',') as genres, 
                                  avg(r.rating) as rating,
                                  userRating.rating as userRating
                           FROM Movies mvs
                            LEFT JOIN genres g
                                ON mvs.id = g.movie_id
                            LEFT JOIN ratings r
                                ON mvs.id = r.movie_id
                            LEFT JOIN ratings userRating
                                ON mvs.id = r.movie_id
                                    and r.user_id = @UserId
                            GROUP BY mvs.id;
                           """;

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var queryResult = await connection.QueryAsync(new CommandDefinition(sql, new {UserId = userId}, cancellationToken: cancellationToken));

        return queryResult.Select(x =>
        {
            Movie? movie = ConvertQueryToMovie(x);
            
            return movie;
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default) => 
        await GetByConditionAsync("mvs.id = @Id", new { Id = id }, userId, cancellationToken);

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null, CancellationToken cancellationToken = default) => 
        await GetByConditionAsync("mvs.slug = @Slug", new { Slug = slug }, userId, cancellationToken);

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        const string insertMovieCommand = """
                                          INSERT INTO movies (id, title, description, slug, year_of_release)
                                          VALUES (@Id, @Title, @Description, @Slug, @YearOfRelease);
                                          """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(insertMovieCommand, movie, cancellationToken: cancellationToken));

        if (result < 1)
        {
            transaction.Rollback();
            return false;
        }

        result = await connection.ExecuteAsync(new CommandDefinition(InsertGenreCommand,
            movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre }), cancellationToken: cancellationToken));

        if (result < 1)
        {
            transaction.Rollback();
            return false;
        }
        
        transaction.Commit();
        
        return true;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        const string updateMovieCommand = """
                                          UPDATE movies 
                                          SET 
                                            title = @Title,
                                            slug = @Slug,
                                            description = @Description,
                                            year_of_release = @YearOfRelease
                                          WHERE id = @Id
                                          """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition(DeleteGenresCommand, new { MovieId = movie.Id }, cancellationToken: cancellationToken));
        
        await connection.ExecuteAsync(new CommandDefinition(InsertGenreCommand, movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre }), cancellationToken: cancellationToken));
        
        var result = await connection.ExecuteAsync(new CommandDefinition(updateMovieCommand, movie, cancellationToken: cancellationToken));
        
        transaction.Commit();
        
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string deleteMovieCommand = "DELETE FROM movies WHERE id = @Id;";
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(DeleteGenresCommand, new { MovieId = id });
        
        var result = await connection.ExecuteAsync(new CommandDefinition(deleteMovieCommand, new { Id = id }, cancellationToken: cancellationToken));
            
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string query = "SELECT count(1) FROM Movies WHERE Id = @Id";
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var result = await connection.QuerySingleOrDefaultAsync<int>(new CommandDefinition(query, new { Id = id }, cancellationToken: cancellationToken));
        
        return result > 0;
    }

    private async Task<Movie?> GetByConditionAsync(string sqlCondition, object parameters, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var sql = $"""
                   SELECT mvs.*, string_agg(g.name, ',') as genres 
                   FROM Movies mvs
                   LEFT JOIN genres g ON mvs.id = g.movie_id
                   WHERE {sqlCondition}
                   GROUP BY mvs.id;
                   """;
        
        ArgumentException.ThrowIfNullOrEmpty(sqlCondition, nameof(sqlCondition));
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var queryResult = await connection.QuerySingleOrDefaultAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        
        return ConvertQueryToMovie(queryResult);
    }

    private static Movie? ConvertQueryToMovie(dynamic? queryResult)
    {
        if (queryResult == null)
            return null;

        var movie = new Movie(queryResult.id)
        {
            Title = queryResult.title,
            Description = queryResult.description,
            YearOfRelease = queryResult.year_of_release,
            UserRating = (byte?) queryResult.user_rating,
            Rating = (float?) queryResult.rating
        };
        
        movie.Genres.AddRange(queryResult.genres.Split(','));

        return movie;
    }
}