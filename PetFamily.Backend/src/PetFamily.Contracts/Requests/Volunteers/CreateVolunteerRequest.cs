using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Volunteer;

namespace PetFamily.Contracts.Requests.Volunteers;

public record CreateVolunteerRequest(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber,
    IEnumerable<SocialNetworkDto> SocialNetworks,
    IEnumerable<HelpRequisiteDto> HelpRequisites);