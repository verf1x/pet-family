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

    public CreateVolunteerHandler(
        IVolunteerRepository volunteersRepository)
    {
        _volunteersRepository = volunteersRepository;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(
        CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.Email).Value;
        
        var phoneNumber = PhoneNumber.Create(request.PhoneNumber).Value;

        var volunteerByEmail = 
            await _volunteersRepository.GetByEmailAsync(email, cancellationToken);
        
        var volunteerByPhoneNumber = 
            await _volunteersRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);
        
        if (volunteerByEmail.IsSuccess || volunteerByPhoneNumber.IsSuccess)
            return Errors.Module.AlreadyExists();
        
        var id = VolunteerId.CreateNew();
        
        var fullName = FullName.Create(
            request.FullName.FirstName,
            request.FullName.LastName,
            request.FullName.MiddleName!).Value;
        
        var description = Description.Create(request.Description).Value;

        var experience = Experience.Create(request.ExperienceYears).Value;

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
            fullName, 
            email,
            description,
            experience,
            phoneNumber,
            socialNetworks, 
            helpRequisites);
        
        
        await _volunteersRepository.AddAsync(volunteer, cancellationToken);

        return (Guid)volunteer.Id;
    }
}