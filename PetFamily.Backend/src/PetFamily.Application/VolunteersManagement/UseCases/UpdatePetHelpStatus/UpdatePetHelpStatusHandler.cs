using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.VolunteersManagement.Enums;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;

public class UpdatePetHelpStatusHandler : ICommandHandler<int, UpdatePetHelpStatusCommand>
{
    private readonly IValidator<UpdatePetHelpStatusCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IUnitOfWork _unitOfWork;

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
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petId = PetId.Create(command.PetId);

        var petResult = volunteerResult.Value.GetPetById(petId);
        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();

        var helpStatus = (HelpStatus)command.HelpStatus;

        petResult.Value.UpdateHelpStatus(helpStatus);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return command.HelpStatus;
    }
}