using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Contracts.Dtos.Species;
using PetFamily.Contracts.Dtos.Volunteer;

namespace PetFamily.Application.Database;

public interface IReadDbContext
{
    IQueryable<VolunteerDto> Volunteers { get; }
    
    IQueryable<PetDto> Pets { get; }
    
    IQueryable<SpeciesDto> Species { get; }
}