using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Dtos;

namespace PetFamily.Application.Database;

public interface IReadDbContext
{
    DbSet<VolunteerDto> Volunteers { get; }
    
    DbSet<PetDto> Pets { get; }
    
    DbSet<SpeciesDto> Species { get; }
}