using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Abstractions;
using PetFamily.Framework.Database;
using PetFamily.Framework.EntityIds;
using PetFamily.Framework.Extensions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;

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
        if (!validationResult.IsValid)
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