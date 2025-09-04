using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using Volunteers.Application.Database;
using Volunteers.Application.VolunteersManagement;
using Volunteers.Infrastructure.Postgres.Database;
using Volunteers.Infrastructure.Postgres.DbContexts;

namespace Volunteers.Infrastructure.Postgres;

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
        services.AddScoped<IUnitOfWork, VolunteersUnitOfWork>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IVolunteersRepository, VolunteersRepository>();

        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<VolunteersWriteDbContext>(_ =>
            new VolunteersWriteDbContext(configuration.GetConnectionString(Constants.Database)!));

        services.AddScoped<IVolunteersReadDbContext, VolunteersReadDbContext>(_ =>
            new VolunteersReadDbContext(configuration.GetConnectionString(Constants.Database)!));

        return services;
    }
}