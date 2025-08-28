using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.SpeciesManagement;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.Application.IntegrationTests.Species;

public class SpeciesTestBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestsWebFactory Factory;
    protected readonly Fixture Fixture;
    protected readonly IServiceScope Scope;
    protected readonly WriteDbContext WriteDbContext;
    protected readonly ISpeciesRepository SpeciesRepository;

    protected SpeciesTestBase(IntegrationTestsWebFactory factory)
    {
        Factory = factory;
        Fixture = new Fixture();
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        SpeciesRepository = Scope.ServiceProvider.GetRequiredService<ISpeciesRepository>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}