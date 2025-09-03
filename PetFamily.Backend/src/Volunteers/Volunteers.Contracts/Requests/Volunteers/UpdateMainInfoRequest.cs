using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Contracts.Requests.Volunteers;

public record UpdateMainInfoRequest(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber);