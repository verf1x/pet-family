using CSharpFunctionalExtensions;
using PetFamily.Framework;
using PetFamily.Framework.EntityIds;
using PetFamily.Framework.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;

namespace Volunteers.Application.VolunteersManagement;

public interface IVolunteersRepository
{
    Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default);

    Task<Guid> RemoveAsync(Volunteer volunteer, CancellationToken cancellationToken = default);

    Task<Result<Volunteer, Error>> GetByIdAsync(VolunteerId volunteerId, CancellationToken cancellationToken = default);

    Task<Result<Volunteer>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<Result<Volunteer>> GetByPhoneNumberAsync(
        PhoneNumber phoneNumber,
        CancellationToken cancellationToken = default);
}