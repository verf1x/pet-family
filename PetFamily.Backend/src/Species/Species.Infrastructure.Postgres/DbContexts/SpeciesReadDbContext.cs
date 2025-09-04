using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Species.Application.Database;
using Species.Contracts.Dtos.Species;

namespace Species.Infrastructure.Postgres.DbContexts;

public class SpeciesReadDbContext(string connectionString) : DbContext, ISpeciesReadDbContext
{
    public IQueryable<SpeciesDto> Species => Set<SpeciesDto>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await base.SaveChangesAsync(cancellationToken);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        optionsBuilder.EnableSensitiveDataLogging();

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ISpeciesReadDbContext).Assembly,
            type => type.FullName?.Contains(nameof(Configurations.Read)) ?? false);

    private ILoggerFactory CreateLoggerFactory()
        => LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
}