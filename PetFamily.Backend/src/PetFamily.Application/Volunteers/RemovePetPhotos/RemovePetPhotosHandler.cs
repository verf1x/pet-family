using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Files;
using PetFamily.Application.Volunteers.UploadPetPhotos;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Volunteers.RemovePetPhotos;

public class RemovePetPhotosHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly IApplicationDbContext _dbContext;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IValidator<RemovePetPhotosCommand> _validator;
    private readonly ILogger<UploadPetPhotosHandler> _logger;

    public RemovePetPhotosHandler(
        IFileProvider fileProvider,
        IApplicationDbContext dbContext,
        IVolunteersRepository volunteersRepository,
        IValidator<RemovePetPhotosCommand> validator,
        ILogger<UploadPetPhotosHandler> logger)
    {
        _fileProvider = fileProvider;
        _dbContext = dbContext;
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<Result<List<string>, ErrorList>> HandleAsync(
        RemovePetPhotosCommand command,
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

        var petPhotos = new List<Photo>();
        foreach (var photoName in command.PhotoNames)
        {
            var photoPathResult = PhotoPath.Create(photoName);
            if (photoPathResult.IsFailure)
                return photoPathResult.Error.ToErrorList();
            var photo = new Photo(photoPathResult.Value);
            
            petPhotos.Add(photo);
        }
        
        var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);
        
        try
        {
            petResult.Value.RemovePhotos(petPhotos);

            _dbContext.Volunteers.Attach(volunteerResult.Value);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            var removeResult = await _fileProvider.RemovePhotosAsync(command.PhotoNames, cancellationToken);
            if (removeResult.IsFailure)
                return removeResult.Error.ToErrorList();
            
            await transaction.CommitAsync(cancellationToken);

            return removeResult.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}