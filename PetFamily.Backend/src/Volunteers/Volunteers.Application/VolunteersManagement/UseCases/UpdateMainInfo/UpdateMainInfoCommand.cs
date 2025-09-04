using PetFamily.Core.Abstractions;
using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdateMainInfo;

public record UpdateMainInfoCommand(
    Guid VolunteerId,
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber) : ICommand;