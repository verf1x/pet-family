using PetFamily.Framework.Abstractions;
using Volunteers.Contracts.Dtos;
using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Application.VolunteersManagement.UseCases.Create;

public record CreateVolunteerCommand(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber,
    IEnumerable<SocialNetworkDto> SocialNetworks,
    IEnumerable<HelpRequisiteDto> HelpRequisites) : ICommand;