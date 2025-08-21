using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.Files;
using PetFamily.Application.Messaging;
using PetFamily.Application.VolunteersManagement.UseCases.Delete;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;

namespace PetFamily.Application.VolunteersManagement.UseCases.HardDeletePet;

public class HardDeletePetHandler : ICommandHandler<Guid, HardDeletePetCommand>
{
    private readonly IValidator<HardDeletePetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileProvider _fileProvider;

    public HardDeletePetHandler(
        IValidator<HardDeletePetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<HardDeleteVolunteerHandler> logger,
        IUnitOfWork unitOfWork,
        IFileProvider fileProvider)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _fileProvider = fileProvider;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        HardDeletePetCommand command,
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

        var pet = petResult.Value;

        var photosPaths = pet.Photos.Select(file => file.Path);

        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            volunteerResult.Value.RemovePet(pet);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var removeFilesResult =
                await _fileProvider.RemoveFilesAsync(photosPaths, cancellationToken);

            if (removeFilesResult.IsFailure)
                return removeFilesResult.Error.ToErrorList();

            transaction.Commit();

            _logger.LogInformation(
                "Pet with ID {PetId} has been hard deleted from volunteer with ID {VolunteerId}.",
                pet.Id,
                volunteerId);

            return pet.Id.Value;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(
                ex,
                "An error occurred while hard deleting pet with ID {PetId} from volunteer with ID {VolunteerId}.",
                pet.Id,
                volunteerId);

            return Error.Failure(
                    "volunteer.pet.hard_delete.failure",
                    "An error occurred while hard deleting the pet with ID " + petId)
                .ToErrorList();
        }
    }
}