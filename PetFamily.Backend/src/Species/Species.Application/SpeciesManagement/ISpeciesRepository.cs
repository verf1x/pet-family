using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Application.SpeciesManagement;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(
        Domain.SpeciesManagement.Species species,
        CancellationToken cancellationToken = default);

    Task<Result<Domain.SpeciesManagement.Species>> GetByNameAsync(
        Name name,
        CancellationToken cancellationToken = default);

    Task<Result<Domain.SpeciesManagement.Species, Error>> GetByIdAsync(
        SpeciesId speciesId,
        CancellationToken cancellationToken = default);

    Task<Guid> RemoveAsync(
        Domain.SpeciesManagement.Species species,
        CancellationToken cancellationToken = default);
}