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
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.VolunteersManagement.UseCases.RemovePetPhotos;

public class RemovePetPhotosHandler : ICommandHandler<List<string>, RemovePetPhotosCommand>
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<RemovePetPhotosHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RemovePetPhotosCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public RemovePetPhotosHandler(
        IFileProvider fileProvider,
        IUnitOfWork unitOfWork,
        IVolunteersRepository volunteersRepository,
        IValidator<RemovePetPhotosCommand> validator,
        ILogger<RemovePetPhotosHandler> logger)
    {
        _fileProvider = fileProvider;
        _unitOfWork = unitOfWork;
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<List<string>, ErrorList>> HandleAsync(
        RemovePetPhotosCommand command,
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

        List<Photo> petPhotos = new();
        foreach (string path in command.PhotoPaths)
        {
            Photo photo = new(path);

            petPhotos.Add(photo);
        }

        IDbTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            petResult.Value.RemovePhotos(petPhotos);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            Result<List<string>, Error> removeResult =
                await _fileProvider.RemoveFilesAsync(command.PhotoPaths, cancellationToken);
            if (removeResult.IsFailure)
            {
                return removeResult.Error.ToErrorList();
            }

            transaction.Commit();

            return removeResult.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while adding pet photos for pet with id {PetId} of volunteer {VolunteerId}",
                petId,
                volunteerId);

            transaction.Rollback();

            return Error.Failure(
                "volunteer.pet.remove_photos.failure",
                "An error occurred while removing photos for pet with id" + petId).ToErrorList();
        }
    }
}