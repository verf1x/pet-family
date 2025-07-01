using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoHandler
{
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly ILogger<UpdateMainInfoHandler> _logger;

    public UpdateMainInfoHandler(
        IVolunteerRepository volunteerRepository,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(
        UpdateMainInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var volunteerId = VolunteerId.Create(request.VolunteerId);
        var volunteerResult = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;
        
        var fullName = FullName.Create(
            request.Dto.FullName.FirstName,
            request.Dto.FullName.LastName,
            request.Dto.FullName.MiddleName).Value;
        
        var email = Email.Create(request.Dto.Email).Value;
        var description = Description.Create(request.Dto.Description).Value;
        var experienceYears = Experience.Create(request.Dto.ExperienceYears).Value;
        var phoneNumber = PhoneNumber.Create(request.Dto.PhoneNumber).Value;
        
        volunteerResult.Value.UpdateMainInfo(
            fullName,
            email,
            description,
            experienceYears,
            phoneNumber);
        
        var result = await _volunteerRepository.SaveAsync(volunteerResult.Value, cancellationToken);
        
        return result;
    }
} 