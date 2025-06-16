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
    
    public async Task<Result<Guid, Error>> HandleAsync(CreateVolunteerRequest request, CancellationToken token = default)
    {
        var emailResult = Email.Create(request.Email);
        
        if (emailResult.IsFailure)
            return emailResult.Error;

        var volunteerToCreate = 
            await _volunteersRepository.GetByEmailAsync(emailResult.Value);
        
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
        
        var socialNetworksResult = request.SocialNetworks.Select(
            sn => SocialNetwork.Create(sn.Name, sn.Url)).ToList();
        
        if(socialNetworksResult.Any(r => r.IsFailure))
            return Errors.General.ValueIsRequired("name or description in social network");
        
        var socialNetworks = SocialNetworks.Create(
            socialNetworksResult.Select(r => r.Value).ToList()).Value;
        
        var helpRequisitesResult = request.HelpRequisites.Select(
            r => HelpRequisite.Create(r.Name, r.Description)).ToList();
        
        if(helpRequisitesResult.Any(r => r.IsFailure))
            return Errors.General.ValueIsRequired("name or description in requisite");
        
        var helpRequisites = HelpRequisites.Create(
            helpRequisitesResult.Select(r => r.Value).ToList()).Value;
        
        Volunteer volunteer = new(
            id,
            fullNameResult.Value, 
            emailResult.Value,
            descriptionResult.Value,
            experienceResult.Value,
            phoneNumberResult.Value,
            socialNetworks, 
            helpRequisites);
        
        
        await _volunteersRepository.AddAsync(volunteer, token);

        return (Guid)volunteer.Id;
    }
}