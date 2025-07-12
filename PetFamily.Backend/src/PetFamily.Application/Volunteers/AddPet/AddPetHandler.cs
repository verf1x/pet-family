using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Volunteers.Entities;
using PetFamily.Domain.Volunteers.ValueObjects;
using File = PetFamily.Domain.Volunteers.ValueObjects.File;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetHandler
{
    private const string BucketName = "photos";

    private readonly IFileProvider _fileProvider;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddPetHandler> _logger;

    public AddPetHandler(
        IFileProvider fileProvider,
        IVolunteersRepository volunteersRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddPetHandler> logger)
    {
        _fileProvider = fileProvider;
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> HandleAsync(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var volunteerResult = await _volunteersRepository
                .GetByIdAsync(VolunteerId.Create(command.VolunteerId), cancellationToken);

            if (volunteerResult.IsFailure)
                return volunteerResult.Error;

            var petId = PetId.CreateNew();
            var nickname = Nickname.Create(command.Nickname).Value;
            var description = Description.Create(command.Description).Value;
            var speciesBreed = SpeciesBreed.Create(
                SpeciesId.Create(command.SpeciesBreedDto.SpeciesId),
                BreedId.Create(command.SpeciesBreedDto.BreedId)).Value;

            var color = Color.Create(command.Color).Value;
            var healthInfo = HealthInfo.Create(
                command.HealthInfoDto.HealthStatus,
                command.HealthInfoDto.IsNeutered,
                command.HealthInfoDto.IsVaccinated).Value;

            var address = Address.Create(
                command.AddressDto.AddressLines.ToList(),
                command.AddressDto.Locality,
                command.AddressDto.Region,
                command.AddressDto.PostalCode,
                command.AddressDto.CountryCode).Value;

            var measurements = Measurements.Create(
                command.MeasurementsDto.Height,
                command.MeasurementsDto.Weight).Value;

            var ownerPhoneNumber = PhoneNumber.Create(command.OwnerPhoneNumber).Value;
            var dateOfBirth = command.DateOfBirth;
            var helpStatus = command.HelpStatus;

            var helpRequisites = command.HelpRequisites
                .Select(r => HelpRequisite.Create(r.Name, r.Description).Value)
                .ToList();

            var filesData = GetFilesData(command);
            if (filesData.IsFailure)
                return filesData.Error;

            var petFiles = GetFilesFromFilesData(filesData.Value);

            var pet = new Pet(
                petId,
                nickname,
                description,
                speciesBreed,
                color,
                healthInfo,
                address,
                measurements,
                ownerPhoneNumber,
                dateOfBirth,
                helpStatus,
                helpRequisites,
                petFiles);

            volunteerResult.Value.AddPet(pet);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var uploadResult = await _fileProvider.UploadFilesAsync(filesData.Value, cancellationToken);
            if (uploadResult.IsFailure)
                return uploadResult.Error;
            
            transaction.Commit();

            return pet.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot add pet for volunteer with id {VolunteerId}", command.VolunteerId);
            
            transaction.Rollback();
            
            return Error.Failure(
                    "volunteer.pet.failure",
                    "Cannot add pet for volunteer with id " + command.VolunteerId);
        }
    }

    private Result<List<FileData>, Error> GetFilesData(AddPetCommand command)
    {
        var result = new List<FileData>();

        foreach (var file in command.Files)
        {
            var extension = Path.GetExtension(file.FileName);

            var pathResult = FilePath.Create(Guid.NewGuid(), extension);
            if (pathResult.IsFailure)
                return pathResult.Error;

            var fileContent = new FileData(file.Content, pathResult.Value, BucketName);
            result.Add(fileContent);
        }

        return result;
    }

    private List<File> GetFilesFromFilesData(List<FileData> filesData)
    {
        return filesData
            .Select(f => f.FilePath)
            .Select(f => new File(f))
            .ToList();
    }
}