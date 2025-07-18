using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Domain.SpeciesManagement;
using PetFamily.Domain.VolunteersManagement.Entities;

namespace PetFamily.Application.Database;

public interface IApplicationDbContext
{
    DbSet<Volunteer> Volunteers { get; }
    
    DbSet<Species> Species { get; }
    
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}