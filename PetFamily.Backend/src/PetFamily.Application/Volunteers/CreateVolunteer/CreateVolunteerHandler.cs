using CSharpFunctionalExtensions;
using PetFamily.Domain.Entities;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects;
using PetFamily.Domain.ValueObjects.Pet;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteerRepository _volunteersRepository;

    public CreateVolunteerHandler(IVolunteerRepository volunteersRepository)
    {
        _volunteersRepository = volunteersRepository;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(CreateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(request.Email);
        
        if (emailResult.IsFailure)
            return emailResult.Error;

        var volunteerToCreate = 
            await _volunteersRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        
        if (volunteerToCreate.IsSuccess)
            return Errors.Module.AlreadyExists();
        
        var id = VolunteerId.CreateNew();
        
        var fullNameResult = FullName.Create(
            request.FullName.FirstName,
            request.FullName.LastName,
            request.FullName.MiddleName!);

        if (fullNameResult.IsFailure)
            return fullNameResult.Error;
        
        var descriptionResult = Description.Create(request.Description);
        
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        var experienceResult = Experience.Create(request.ExperienceYears);
        
        if (experienceResult.IsFailure)
            return experienceResult.Error;
        
        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
        
        if(phoneNumberResult.IsFailure)
            return phoneNumberResult.Error;

        var socialNetworks = new SocialNetworks(
            request.SocialNetworks
                .Select(s => SocialNetwork.Create(s.Name, s.Url).Value)
                .ToList());
        
        var helpRequisites = new HelpRequisites(
            request.HelpRequisites
                .Select(r => HelpRequisite.Create(r.Name, r.Description).Value)
                .ToList());
        
        Volunteer volunteer = new(
            id,
            fullNameResult.Value, 
            emailResult.Value,
            descriptionResult.Value,
            experienceResult.Value,
            phoneNumberResult.Value,
            socialNetworks, 
            helpRequisites);
        
        
        await _volunteersRepository.AddAsync(volunteer, cancellationToken);

        return (Guid)volunteer.Id;
    }
}