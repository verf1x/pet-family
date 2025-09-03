using Volunteers.Contracts.Dtos.Pet;
using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Application.Database;

public interface IVolunteersReadDbContext
{
    IQueryable<VolunteerDto> Volunteers { get; }

    IQueryable<PetDto> Pets { get; }
}