using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volunteers.Application;
using Volunteers.Infrastructure.Postgres;

namespace Volunteers.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddVolunteersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}