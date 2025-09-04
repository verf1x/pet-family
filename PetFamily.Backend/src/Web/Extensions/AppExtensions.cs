using Microsoft.EntityFrameworkCore;
using Species.Infrastructure.Postgres.DbContexts;
using Volunteers.Infrastructure.Postgres.DbContexts;

namespace Web.Extensions;

public static class AppExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();

        VolunteersWriteDbContext volunteersWriteDbContext =
            scope.ServiceProvider.GetRequiredService<VolunteersWriteDbContext>();
        SpeciesWriteDbContext speciesWriteDbContext = scope.ServiceProvider.GetRequiredService<SpeciesWriteDbContext>();

        await volunteersWriteDbContext.Database.MigrateAsync();
        await speciesWriteDbContext.Database.MigrateAsync();
    }
}