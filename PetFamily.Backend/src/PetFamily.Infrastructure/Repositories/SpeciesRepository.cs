using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.SpeciesManagement;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.SpeciesManagement;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.Infrastructure.Repositories;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly WriteDbContext _writeDbContext;

    public SpeciesRepository(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<Guid> AddAsync(Species species, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.Species.AddAsync(species, cancellationToken);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return species.Id;
    }

    public async Task<Result<Species>> GetByNameAsync(Name name, CancellationToken cancellationToken = default)
    {
        var species = await _writeDbContext.Species
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);

        return species ?? Result.Failure<Species>("Name not found");
    }

    public async Task<Result<Species, Error>> GetByIdAsync(
        SpeciesId speciesId,
        CancellationToken cancellationToken = default)
    {
        var species = await _writeDbContext.Species
            .FirstOrDefaultAsync(s => s.Id == speciesId, cancellationToken);

        if (species is null)
            return Errors.General.NotFound(speciesId);

        return species;
    }

    public async Task<Guid> RemoveAsync(Species species, CancellationToken cancellationToken = default)
    {
        _writeDbContext.Species.Remove(species);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return species.Id;
    }
}