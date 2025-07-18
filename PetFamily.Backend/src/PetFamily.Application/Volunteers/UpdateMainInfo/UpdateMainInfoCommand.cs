using PetFamily.Application.Dtos;
using PetFamily.Application.Dtos.Volunteer;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public record UpdateMainInfoCommand(
    Guid VolunteerId,
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber);