using CSharpFunctionalExtensions;
using PetFamily.Domain.Entities;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Application.Volunteers;

public interface IVolunteerRepository
{
    Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default);
    Task<Result<Volunteer>> GetByIdAsync(VolunteerId volunteerId);
}