using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.VolunteersManagement.UseCases.MovePet;

public class MovePetHandler : ICommandHandler<int, MovePetCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<MovePetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public MovePetHandler(
        IVolunteersRepository volunteersRepository,
        IUnitOfWork unitOfWork,
        IValidator<MovePetCommand> validator)
    {
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<int, ErrorList>> HandleAsync(
        MovePetCommand command,
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

        Result<Position, Error> newPositionResult = Position.Create(command.NewPosition);
        if (newPositionResult.IsFailure)
        {
            return newPositionResult.Error.ToErrorList();
        }

        UnitResult<Error> movePetResult = volunteerResult.Value.MovePet(petResult.Value, newPositionResult.Value);
        if (movePetResult.IsFailure)
        {
            return movePetResult.Error.ToErrorList();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return petResult.Value.Position.Value;
    }
}