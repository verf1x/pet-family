using Species.Presenters;
using Volunteers.Presenters;

namespace Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddWebDependencies();

        services.AddVolunteersModule(configuration);

        services.AddSpeciesModule(configuration);

        return services;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}