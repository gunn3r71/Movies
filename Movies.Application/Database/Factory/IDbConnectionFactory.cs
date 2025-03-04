using System.Data;

namespace Movies.Application.Database.Factory;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}