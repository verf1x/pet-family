using CSharpFunctionalExtensions;
using PetFamily.Domain.SpeciesManagement;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.SpeciesManagement;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(Species species, CancellationToken cancellationToken = default);
    
    Task<Result<Species>> GetByNameAsync(Name name, CancellationToken cancellationToken = default);
}