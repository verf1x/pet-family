using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Database;
using PetFamily.Application.SpeciesManagement;
using PetFamily.Application.VolunteersManagement;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class VolunteerTestBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestsWebFactory Factory;
    protected readonly Fixture Fixture;
    protected readonly IServiceScope Scope;
    protected readonly WriteDbContext WriteDbContext;
    protected readonly ISpeciesRepository SpeciesRepository;
    protected readonly IVolunteersRepository VolunteersRepository;
    protected readonly ISqlConnectionFactory SqlConnectionFactory;

    protected VolunteerTestBase(IntegrationTestsWebFactory factory)
    {
        Factory = factory;
        Fixture = new Fixture();
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
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