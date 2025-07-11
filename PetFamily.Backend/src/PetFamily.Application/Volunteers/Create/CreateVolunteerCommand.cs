using PetFamily.Application.Dtos;

namespace PetFamily.Application.Volunteers.Create;

public record CreateVolunteerCommand(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber,
    IEnumerable<SocialNetworkDto> SocialNetworks,
    IEnumerable<HelpRequisiteDto> HelpRequisites);