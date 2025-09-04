using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Core.Database;
using Species.Infrastructure.Postgres.DbContexts;

namespace Species.Infrastructure.Postgres.Database;

public class SpeciesUnitOfWork : IUnitOfWork
{
    private readonly SpeciesWriteDbContext _writeDbContext;

    public SpeciesUnitOfWork(SpeciesWriteDbContext writeDbContext) => _writeDbContext = writeDbContext;

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await _writeDbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _writeDbContext.SaveChangesAsync(cancellationToken);
}