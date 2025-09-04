using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;

public class UpdatePetHelpStatusHandler : ICommandHandler<int, UpdatePetHelpStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdatePetHelpStatusCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public UpdatePetHelpStatusHandler(
        IValidator<UpdatePetHelpStatusCommand> validator,
        IVolunteersRepository volunteersRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int, ErrorList>> HandleAsync(
        UpdatePetHelpStatusCommand command,
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

        PetId petId = PetId.Create(command.PetId);

        Result<Pet, Error> petResult = volunteerResult.Value.GetPetById(petId);
        if (petResult.IsFailure)
        {
            return petResult.Error.ToErrorList();
        }

        HelpStatus helpStatus = (HelpStatus)command.HelpStatus;

        petResult.Value.UpdateHelpStatus(helpStatus);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return command.HelpStatus;
    }
}