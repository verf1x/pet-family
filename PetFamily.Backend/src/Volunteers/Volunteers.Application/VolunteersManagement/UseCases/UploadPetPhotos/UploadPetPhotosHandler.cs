using System.Data;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.Core.Files;
using PetFamily.Core.Messaging;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;
using Volunteers.Application.Extensions;

namespace Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;

public class UploadPetPhotosHandler : ICommandHandler<List<string>, UploadPetPhotosCommand>
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<UploadPetPhotosHandler> _logger;
    private readonly IMessageQueue<IEnumerable<string>> _messageQueue;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UploadPetPhotosCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public UploadPetPhotosHandler(
        IFileProvider fileProvider,
        IUnitOfWork unitOfWork,
        IVolunteersRepository volunteersRepository,
        IValidator<UploadPetPhotosCommand> validator,
        IMessageQueue<IEnumerable<string>> messageQueue,
        ILogger<UploadPetPhotosHandler> logger)
    {
        _fileProvider = fileProvider;
        _unitOfWork = unitOfWork;
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _messageQueue = messageQueue;
        _logger = logger;
    }

    public async Task<Result<List<string>, ErrorList>> HandleAsync(
        UploadPetPhotosCommand command,
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

        Result<List<PhotoData>, Error> filesData = command.Photos.ToDataCollection();
        if (filesData.IsFailure)
        {
            return filesData.Error.ToErrorList();
        }

        List<Photo> petPhotos = filesData.Value.ToFilesCollection();

        IDbTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            petResult.Value.AddPhotos(petPhotos);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            Result<List<string>, Error> uploadResult =
                await _fileProvider.UploadPhotosAsync(filesData.Value, cancellationToken);
            if (uploadResult.IsFailure)
            {
                await _messageQueue.WriteAsync(
                    filesData.Value.Select(f => f.Path),
                    cancellationToken);

                return uploadResult.Error.ToErrorList();
            }

            transaction.Commit();

            List<string> photoPaths = uploadResult.Value
                .Select(file => file)
                .ToList();

            return photoPaths;
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
                "volunteer.pet.add_photos.failure",
                "An error occurred while adding photos for pet with id" + petId).ToErrorList();
        }
    }
}