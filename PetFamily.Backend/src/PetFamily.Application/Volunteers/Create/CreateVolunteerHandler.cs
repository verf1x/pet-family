using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Volunteers.Entities;
using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Application.Volunteers.Create;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersesRepository;
    private readonly ILogger<CreateVolunteerHandler> _logger;

    public CreateVolunteerHandler(
        IVolunteersRepository volunteersesRepository,
        ILogger<CreateVolunteerHandler> logger)
    {
        _volunteersesRepository = volunteersesRepository;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(
        CreateVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = Email.Create(command.Email).Value;
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;

        var volunteerByEmail = 
            await _volunteersesRepository.GetByEmailAsync(email, cancellationToken);
        
        var volunteerByPhoneNumber = 
            await _volunteersesRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);
        
        if (volunteerByEmail.IsSuccess || volunteerByPhoneNumber.IsSuccess)
            return Errors.Module.AlreadyExists();
        
        var id = VolunteerId.CreateNew();
        
        var fullName = FullName.Create(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.MiddleName!).Value;
        
        var description = Description.Create(command.Description).Value;
        var experience = Experience.Create(command.ExperienceYears).Value;
        
        var socialNetworks = new SocialNetworks(
            command.SocialNetworks
                .Select(s => SocialNetwork.Create(s.Name, s.Url).Value)
                .ToList());
        
        var helpRequisites = new HelpRequisites(
            command.HelpRequisites
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
        
        await _volunteersesRepository.AddAsync(volunteer, cancellationToken);
        
        _logger.LogInformation("Created volunteer with ID: {id}", id);

        return (Guid)volunteer.Id;
    }
}