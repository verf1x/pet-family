using System.Data;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.Core.Files;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

namespace Volunteers.Application.VolunteersManagement.UseCases.HardDeletePet;

public class HardDeletePetHandler : ICommandHandler<Guid, HardDeletePetCommand>
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<HardDeletePetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

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

        Pet? pet = petResult.Value;

        IEnumerable<string> photosPaths = pet.Photos.Select(file => file.Path);

        IDbTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            volunteerResult.Value.RemovePet(pet);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            Result<List<string>, Error> removeFilesResult =
                await _fileProvider.RemoveFilesAsync(photosPaths, cancellationToken);

            if (removeFilesResult.IsFailure)
            {
                return removeFilesResult.Error.ToErrorList();
            }

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