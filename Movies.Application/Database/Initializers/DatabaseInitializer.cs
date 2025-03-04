using Dapper;
using Movies.Application.Database.Factory;

namespace Movies.Application.Database.Initializers;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS movies(
                                        id UUID NOT NULL PRIMARY KEY,
                                        title TEXT NOT NULL,
                                        description TEXT NOT NULL,
                                        slug TEXT NOT NULL,
                                        year_of_release INTEGER NOT NULL,
                                        genres TEXT[] NOT NULL
                                      );
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX CONCURRENTLY
                                        IF NOT EXISTS movies_slug_idx 
                                        ON movies
                                        using btree(slug);
                                      """);
    }
}