using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Framework;
using PetFamily.Framework.EntityIds;
using Species.Application.SpeciesManagement;
using Species.Domain.SpeciesManagement.ValueObjects;
using Species.Infrastructure.Postgres.DbContexts;

namespace Species.Infrastructure.Postgres;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly SpeciesWriteDbContext _writeDbContext;

    public SpeciesRepository(SpeciesWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<Guid> AddAsync(Species.Domain.SpeciesManagement.Species species, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.Species.AddAsync(species, cancellationToken);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return species.Id;
    }

    public async Task<Result<Species.Domain.SpeciesManagement.Species>> GetByNameAsync(Name name, CancellationToken cancellationToken = default)
    {
        var species = await _writeDbContext.Species
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);

        return species ?? Result.Failure<Species.Domain.SpeciesManagement.Species>("Name not found");
    }

    public async Task<Result<Species.Domain.SpeciesManagement.Species, Error>> GetByIdAsync(
        SpeciesId speciesId,
        CancellationToken cancellationToken = default)
    {
        var species = await _writeDbContext.Species
            .FirstOrDefaultAsync(s => s.Id == speciesId, cancellationToken);

        if (species is null)
            return Errors.General.NotFound(speciesId);

        return species;
    }

    public async Task<Guid> RemoveAsync(Species.Domain.SpeciesManagement.Species species, CancellationToken cancellationToken = default)
    {
        _writeDbContext.Species.Remove(species);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return species.Id;
    }
}