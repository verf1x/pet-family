using CSharpFunctionalExtensions;
using PetFamily.Domain.Entities;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Pet;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteerRepository _volunteerRepository;

    public CreateVolunteerHandler(IVolunteerRepository volunteerRepository)
    {
        _volunteerRepository = volunteerRepository;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(CreateVolunteerRequest request, CancellationToken token = default)
    {
        var id = VolunteerId.CreateNew();
        
        var fullNameResult = FullName.Create(request.FirstName, request.LastName);

        if (fullNameResult.IsFailure)
            return fullNameResult.Error;
        
        var emailResult = Email.Create(request.Email);
        
        if (emailResult.IsFailure)
            return emailResult.Error;
        
        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
        
        if(phoneNumberResult.IsFailure)
            return phoneNumberResult.Error;

        SocialNetworks socialNetworks = new();
        
        HelpRequisites helpRequisites = new();
        
        var volunteerResult = Volunteer.Create(
            id,
            fullNameResult.Value, 
            emailResult.Value,
            request.Description,
            request.ExperienceYears,
            phoneNumberResult.Value,
            socialNetworks,
            helpRequisites);
        
        if(volunteerResult.IsFailure)
            return volunteerResult.Error;
        
        await _volunteerRepository.AddAsync(volunteerResult.Value, token);

        return (Guid)volunteerResult.Value.Id;
    }
}