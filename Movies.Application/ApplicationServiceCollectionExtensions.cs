using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database.Factory;
using Movies.Application.Database.Initializers;
using Movies.Application.Repositories;

namespace Movies.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IMoviesRepository, MoviesInMemoryRepository>();
            
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>(_ =>
                new NpgsqlDbConnectionFactory(connectionString));

            services.AddSingleton<DatabaseInitializer>();
            
            return services;
        }
    }
}
