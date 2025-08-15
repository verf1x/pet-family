using PetFamily.Contracts.Dtos.Volunteer;

namespace PetFamily.Contracts.Requests.Volunteers;

public record UpdateMainInfoRequest(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber);