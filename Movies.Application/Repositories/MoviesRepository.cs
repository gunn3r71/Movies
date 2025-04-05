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

    public async Task<IEnumerable<Movie>> GetAsync()
    {
        const string sql = """
                           SELECT mvs.*, string_agg(g.name, ',') as genres FROM Movies mvs
                            LEFT JOIN genres g
                                ON mvs.Id = g.movie_id
                            GROUP BY mvs.id;
                           """;

        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var queryResult = await connection.QueryAsync(sql);

        return queryResult.Select(x =>
        {
            Movie? movie = ConvertQueryToMovie(x);
            
            return movie;
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id) => 
        await GetByConditionAsync("mvs.id", new { id = id });

    public async Task<Movie?> GetBySlugAsync(string slug) => 
        await GetByConditionAsync("mvs.slug", new { slug = slug });

    public async Task<bool> CreateAsync(Movie movie)
    {
        const string insertMovieCommand = """
                                          INSERT INTO movies (id, title, description, slug, year_of_release)
                                          VALUES (@Id, @Title, @Description, @Slug, @YearOfRelease);
                                          """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync();

        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(insertMovieCommand, movie));

        if (result < 1)
        {
            transaction.Rollback();
            return false;
        }

        result = await connection.ExecuteAsync(InsertGenreCommand,
            movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre }));

        if (result < 1)
        {
            transaction.Rollback();
            return false;
        }
        
        transaction.Commit();
        
        return true;
    }

    public async Task<bool> UpdateAsync(Movie movie)
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
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition(DeleteGenresCommand, new { MovieId = movie.Id }));
        
        await connection.ExecuteAsync(new CommandDefinition(InsertGenreCommand, movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre })));
        
        var result = await connection.ExecuteAsync(new CommandDefinition(updateMovieCommand, movie));
        
        transaction.Commit();
        
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string deleteMovieCommand = "DELETE FROM movies WHERE id = @Id;";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(DeleteGenresCommand, new { MovieId = id });
        
        var result = await connection.ExecuteAsync(new CommandDefinition(deleteMovieCommand, new { Id = id }));
            
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        const string query = "SELECT count(1) FROM Movies WHERE Id = @Id";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var result = await connection.QuerySingleOrDefaultAsync<int>(new CommandDefinition(query, new { Id = id }));
        
        return result > 0;
    }

    private async Task<Movie?> GetByConditionAsync(string sqlCondition, object parameters)
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
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var queryResult = await connection.QuerySingleOrDefaultAsync(sql, parameters);
        
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
            YearOfRelease = queryResult.year_of_release
        };
        
        movie.Genres.AddRange(queryResult.genres.Split(','));

        return movie;
    }
}