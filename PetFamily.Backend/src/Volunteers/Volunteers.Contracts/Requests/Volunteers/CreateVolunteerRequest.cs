using Volunteers.Contracts.Dtos;
using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Contracts.Requests.Volunteers;

public record CreateVolunteerRequest(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber,
    IEnumerable<SocialNetworkDto> SocialNetworks,
    IEnumerable<HelpRequisiteDto> HelpRequisites);