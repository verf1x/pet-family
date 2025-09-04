using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdateMainInfo;

public class UpdateMainInfoHandler : ICommandHandler<Guid, UpdateMainInfoCommand>
{
    private readonly ILogger<UpdateMainInfoHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMainInfoCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public UpdateMainInfoHandler(
        IVolunteersRepository volunteersRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateMainInfoCommand> validator,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateMainInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId volunteerId = VolunteerId.Create(command.VolunteerId);

        Result<Volunteer, Error> volunteerResult =
            await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            return volunteerResult.Error.ToErrorList();
        }

        FullName? fullName = FullName.Create(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.MiddleName).Value;

        Email? email = Email.Create(command.Email).Value;
        Description? description = Description.Create(command.Description).Value;
        Experience? experienceYears = Experience.Create(command.ExperienceYears).Value;
        PhoneNumber? phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;

        volunteerResult.Value.UpdateMainInfo(
            fullName,
            email,
            description,
            experienceYears,
            phoneNumber);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated volunteer with ID: {VolunteerId}", command.VolunteerId);

        return volunteerId.Value;
    }
}