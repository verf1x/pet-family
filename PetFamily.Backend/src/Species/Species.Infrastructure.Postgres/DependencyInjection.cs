using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using Species.Application.SpeciesManagement;
using Species.Infrastructure.Postgres.Database;
using Species.Infrastructure.Postgres.DbContexts;

namespace Species.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase()
            .AddRepositories()
            .AddDbContexts(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, SpeciesUnitOfWork>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();

        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<SpeciesWriteDbContext>(_ =>
            new SpeciesWriteDbContext(configuration.GetConnectionString(Constants.Database)!));

        services.AddScoped<SpeciesReadDbContext, SpeciesReadDbContext>(_ =>
            new SpeciesReadDbContext(configuration.GetConnectionString(Constants.Database)!));

        return services;
    }
}