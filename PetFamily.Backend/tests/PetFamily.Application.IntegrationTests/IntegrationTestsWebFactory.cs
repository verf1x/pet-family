using System.Data.Common;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using NSubstitute;
using PetFamily.Application.Database;
using PetFamily.Application.Files;
using PetFamily.Domain.Shared;
using PetFamily.Infrastructure;
using PetFamily.Infrastructure.DbContexts;
using Respawn;
using Testcontainers.PostgreSql;

namespace PetFamily.Application.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("pet_family_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner = null!;
    private DbConnection _dbConnection = null!;
    private readonly IFileProvider _fileProviderMock;

    public IntegrationTestsWebFactory()
    {
        _fileProviderMock = Substitute.For<IFileProvider>();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();

        await _dbContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public void SetupFileProviderSuccessUploadMock()
    {
        _fileProviderMock
            .UploadPhotosAsync(Arg.Any<IEnumerable<PhotoData>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<List<string>, Error>(["fake1.jpg", "fake2.jpg"]));
    }

    public void SetupFileProviderFailedUploadMock()
    {
        _fileProviderMock
            .UploadPhotosAsync(Arg.Any<IEnumerable<PhotoData>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<List<string>, Error>(
                Error.Failure("fake.upload.error", "Failed to upload files")));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        services.RemoveAll<IFileProvider>();
        services.AddScoped(_ => _fileProviderMock);

        services.RemoveAll<WriteDbContext>();
        services.RemoveAll<IReadDbContext>();
        services.RemoveAll<ISqlConnectionFactory>();

        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(_dbContainer.GetConnectionString()));

        services.AddScoped<IReadDbContext>(_ =>
            new ReadDbContext(_dbContainer.GetConnectionString()));

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(_dbContainer.GetConnectionString()));
    }

    private async Task InitializeRespawnerAsync()
    {
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres, SchemasToInclude = ["public"], });
    }
}