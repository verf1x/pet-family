using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Species.Application;
using Species.Infrastructure.Postgres;

namespace Species.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddSpeciesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}