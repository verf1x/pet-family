using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Core.Database;
using Volunteers.Infrastructure.Postgres.DbContexts;

namespace Volunteers.Infrastructure.Postgres.Database;

public class VolunteersUnitOfWork : IUnitOfWork
{
    private readonly VolunteersWriteDbContext _writeDbContext;

    public VolunteersUnitOfWork(VolunteersWriteDbContext writeDbContext) => _writeDbContext = writeDbContext;

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await _writeDbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _writeDbContext.SaveChangesAsync(cancellationToken);
}