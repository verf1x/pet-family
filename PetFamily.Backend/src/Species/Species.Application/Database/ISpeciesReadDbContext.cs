using Species.Contracts.Dtos.Species;

namespace Species.Application.Database;

public interface ISpeciesReadDbContext
{
    IQueryable<SpeciesDto> Species { get; }
}