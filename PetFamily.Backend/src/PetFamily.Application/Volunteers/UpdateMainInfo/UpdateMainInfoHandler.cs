using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoHandler
{
    private readonly IVolunteerRepository _volunteersRepository;
    private readonly ILogger<UpdateMainInfoHandler> _logger;

    public UpdateMainInfoHandler(
        IVolunteerRepository volunteersRepository,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(
        UpdateMainInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;
        
        var fullName = FullName.Create(
            command.Dto.FullName.FirstName,
            command.Dto.FullName.LastName,
            command.Dto.FullName.MiddleName).Value;
        
        var email = Email.Create(command.Dto.Email).Value;
        var description = Description.Create(command.Dto.Description).Value;
        var experienceYears = Experience.Create(command.Dto.ExperienceYears).Value;
        var phoneNumber = PhoneNumber.Create(command.Dto.PhoneNumber).Value;
        
        volunteerResult.Value.UpdateMainInfo(
            fullName,
            email,
            description,
            experienceYears,
            phoneNumber);
        
        var result = await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);
        
        _logger.LogInformation("Updated volunteer with ID: {VolunteerId}", command.VolunteerId); 
        
        return result;
    }
} 