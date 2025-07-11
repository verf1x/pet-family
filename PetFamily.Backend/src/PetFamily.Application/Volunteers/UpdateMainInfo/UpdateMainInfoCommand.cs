using PetFamily.Application.Dtos;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public record UpdateMainInfoCommand(
    Guid VolunteerId, 
    UpdateMainInfoDto Dto);
    
public record UpdateMainInfoDto(
    FullNameDto FullName,
    string Email,
    string Description,
    int ExperienceYears,
    string PhoneNumber);