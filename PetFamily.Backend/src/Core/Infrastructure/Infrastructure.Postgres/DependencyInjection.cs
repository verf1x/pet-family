using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework;
using PetFamily.Framework.Database;

namespace Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>(_ =>
            new SqlConnectionFactory(configuration.GetConnectionString(Constants.Database)!));

        return services;
    }
}