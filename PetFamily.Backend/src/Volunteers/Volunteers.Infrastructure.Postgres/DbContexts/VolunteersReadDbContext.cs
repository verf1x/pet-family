using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volunteers.Application.Database;
using Volunteers.Contracts.Dtos.Pet;
using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Infrastructure.Postgres.DbContexts;

public class VolunteersReadDbContext(string connectionString) : DbContext, IVolunteersReadDbContext
{
    public IQueryable<VolunteerDto> Volunteers => Set<VolunteerDto>();

    public IQueryable<PetDto> Pets => Set<PetDto>();

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

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IVolunteersReadDbContext).Assembly,
            type => type.FullName?.Contains(nameof(Configurations.Read)) ?? false);
    }

    private ILoggerFactory CreateLoggerFactory()
        => LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
}