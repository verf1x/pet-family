using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Database;
using Species.Application.SpeciesManagement;
using Species.Infrastructure.Postgres.DbContexts;
using Volunteers.Application.VolunteersManagement;
using Volunteers.Infrastructure.Postgres.DbContexts;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class VolunteerTestBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestsWebFactory Factory;
    protected readonly Fixture Fixture;
    protected readonly IServiceScope Scope;
    protected readonly VolunteersWriteDbContext VolunteersWriteDbContext;
    protected readonly SpeciesWriteDbContext SpeciesWriteDbContext;
    protected readonly ISpeciesRepository SpeciesRepository;
    protected readonly IVolunteersRepository VolunteersRepository;
    protected readonly ISqlConnectionFactory SqlConnectionFactory;

    protected VolunteerTestBase(IntegrationTestsWebFactory factory)
    {
        Factory = factory;
        Fixture = new Fixture();
        Scope = factory.Services.CreateScope();
        VolunteersWriteDbContext = Scope.ServiceProvider.GetRequiredService<VolunteersWriteDbContext>();
        SpeciesWriteDbContext = Scope.ServiceProvider.GetRequiredService<SpeciesWriteDbContext>();
        SpeciesRepository = Scope.ServiceProvider.GetRequiredService<ISpeciesRepository>();
        VolunteersRepository = Scope.ServiceProvider.GetRequiredService<IVolunteersRepository>();
        SqlConnectionFactory = Scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}