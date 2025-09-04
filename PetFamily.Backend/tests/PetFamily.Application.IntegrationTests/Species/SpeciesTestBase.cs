using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Species.Application.SpeciesManagement;
using Species.Infrastructure.Postgres.DbContexts;

namespace PetFamily.Application.IntegrationTests.Species;

public class SpeciesTestBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestsWebFactory Factory;
    protected readonly Fixture Fixture;
    protected readonly IServiceScope Scope;
    protected readonly ISpeciesRepository SpeciesRepository;
    protected readonly SpeciesWriteDbContext WriteDbContext;

    protected SpeciesTestBase(IntegrationTestsWebFactory factory)
    {
        Factory = factory;
        Fixture = new Fixture();
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<SpeciesWriteDbContext>();
        SpeciesRepository = Scope.ServiceProvider.GetRequiredService<ISpeciesRepository>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}