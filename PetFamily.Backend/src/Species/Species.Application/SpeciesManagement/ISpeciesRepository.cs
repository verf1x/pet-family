using CSharpFunctionalExtensions;
using PetFamily.Framework;
using PetFamily.Framework.EntityIds;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Application.SpeciesManagement;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(
        Species.Domain.SpeciesManagement.Species species,
        CancellationToken cancellationToken = default);

    Task<Result<Species.Domain.SpeciesManagement.Species>> GetByNameAsync(
        Name name,
        CancellationToken cancellationToken = default);

    Task<Result<Species.Domain.SpeciesManagement.Species, Error>> GetByIdAsync(
        SpeciesId speciesId,
        CancellationToken cancellationToken = default);

    Task<Guid> RemoveAsync(
        Species.Domain.SpeciesManagement.Species species,
        CancellationToken cancellationToken = default);
}