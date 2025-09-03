using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Framework;
using PetFamily.Framework.Abstractions;
using PetFamily.Framework.Database;
using PetFamily.Framework.EntityIds;
using PetFamily.Framework.Extensions;

namespace Volunteers.Application.VolunteersManagement.UseCases.SoftDeletePet;

public class SoftDeletePetHandler : ICommandHandler<Guid, SoftDeletePetCommand>
{
    private readonly IValidator<SoftDeletePetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SoftDeletePetHandler> _logger;

    public SoftDeletePetHandler(
        IValidator<SoftDeletePetCommand> validator,
        IVolunteersRepository volunteersRepository,
        IUnitOfWork unitOfWork,
        ILogger<SoftDeletePetHandler> logger)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        SoftDeletePetCommand command,
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

        petResult.Value.SoftDelete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft deleted pet with ID: {PetId}", command.PetId);

        return petId.Value;
    }
}