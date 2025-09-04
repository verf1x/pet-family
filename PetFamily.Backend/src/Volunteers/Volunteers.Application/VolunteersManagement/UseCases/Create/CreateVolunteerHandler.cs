using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.VolunteersManagement.UseCases.Create;

public class CreateVolunteerHandler : ICommandHandler<Guid, CreateVolunteerCommand>
{
    private readonly ILogger<CreateVolunteerHandler> _logger;
    private readonly IValidator<CreateVolunteerCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public CreateVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        IValidator<CreateVolunteerCommand> validator,
        ILogger<CreateVolunteerHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        Email? email = Email.Create(command.Email).Value;
        PhoneNumber? phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;

        Result<Volunteer> volunteerByEmailResult =
            await _volunteersRepository.GetByEmailAsync(email, cancellationToken);

        Result<Volunteer> volunteerByPhoneNumberResult =
            await _volunteersRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

        if (volunteerByEmailResult.IsSuccess || volunteerByPhoneNumberResult.IsSuccess)
        {
            return Errors.Volunteer.AlreadyExists().ToErrorList();
        }

        VolunteerId id = VolunteerId.CreateNew();

        FullName? fullName = FullName.Create(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.MiddleName!).Value;

        Description? description = Description.Create(command.Description).Value;
        Experience? experience = Experience.Create(command.ExperienceYears).Value;

        List<SocialNetwork> socialNetworks = new(
            command.SocialNetworks
                .Select(s => SocialNetwork.Create(s.Name, s.Url).Value)
                .ToList());

        List<HelpRequisite> helpRequisites = new(
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

        Guid result = await _volunteersRepository.AddAsync(volunteer, cancellationToken);

        _logger.LogInformation("Created volunteer with ID: {id}", id);

        return result;
    }
}