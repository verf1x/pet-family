using Volunteers.Contracts.Dtos.Pet;

namespace Volunteers.Contracts.Dtos.Volunteer;

public class VolunteerDto
{
    public Guid Id { get; init; }

    public string Description { get; init; } = null!;

    public int Experience { get; init; }

    public string PhoneNumber { get; init; } = null!;

    public PetDto[] Pets { get; init; } = [];
}