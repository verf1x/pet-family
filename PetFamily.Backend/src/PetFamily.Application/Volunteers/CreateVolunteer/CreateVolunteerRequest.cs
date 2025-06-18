using PetFamily.Application.Dtos;
using PetFamily.Application.DTOs;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public record CreateVolunteerRequest(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber,
    IEnumerable<SocialNetworkDto> SocialNetworks,
    IEnumerable<HelpRequisitesDto> HelpRequisites);