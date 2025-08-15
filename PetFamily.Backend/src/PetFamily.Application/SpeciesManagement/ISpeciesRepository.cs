using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.SpeciesManagement;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.SpeciesManagement;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(Species species, CancellationToken cancellationToken = default);
    
    Task<Result<Species>> GetByNameAsync(Name name, CancellationToken cancellationToken = default);
    
    Task<Result<Species, Error>> GetByIdAsync(SpeciesId speciesId, CancellationToken cancellationToken = default);
    
    Task<Guid> RemoveAsync(Species species, CancellationToken cancellationToken = default);
}