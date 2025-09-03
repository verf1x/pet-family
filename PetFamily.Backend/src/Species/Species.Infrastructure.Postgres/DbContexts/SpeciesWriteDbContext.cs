using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Species.Infrastructure.Postgres.DbContexts;

public class SpeciesWriteDbContext(string connectionString) : DbContext
{
    public DbSet<Domain.SpeciesManagement.Species> Species => Set<Domain.SpeciesManagement.Species>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SpeciesWriteDbContext).Assembly,
            type => type.FullName?.Contains(nameof(Configurations.Write)) ?? false);
    }

    private ILoggerFactory CreateLoggerFactory()
        => LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
}