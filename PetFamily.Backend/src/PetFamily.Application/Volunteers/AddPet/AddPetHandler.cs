using CSharpFunctionalExtensions;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Volunteers.Entities;
using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetHandler
{
    private const string BucketName = "photos";
    
    private readonly IFileProvider _fileProvider;
    private readonly IVolunteerRepository _volunteersRepository;

    public AddPetHandler(IFileProvider fileProvider, IVolunteerRepository volunteersRepository)
    {
        _fileProvider = fileProvider;
        _volunteersRepository = volunteersRepository;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        var volunteerResult = await _volunteersRepository
            .GetByIdAsync(VolunteerId.Create(command.VolunteerId), cancellationToken);
        
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;

        var petId = PetId.CreateNew();
        var nickname = Nickname.Create(command.Nickname).Value;
        var description = Description.Create(command.Description).Value;
        var speciesBreed = SpeciesBreed.Create(
            SpeciesId.CreateEmpty(), //TODO: Заменить на реальный ID вида
            BreedId.CreateEmpty()).Value;
        
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

        var helpRequisites = new HelpRequisites(
            command.HelpRequisites
                .Select(r => HelpRequisite.Create(r.Name, r.Description).Value));

        //TODO: Добавить валидацию
        
        var fileContents = new List<FileContent>();
        
        foreach (var file in command.Files)
        {
            var extension = Path.GetExtension(file.FileName);

            var photoPath = PhotoPath.Create(Guid.NewGuid(), extension);
            if (photoPath.IsFailure)
                return photoPath.Error;
            
            var fileContent = new FileContent(file.Content, photoPath.Value.Path);
            fileContents.Add(fileContent);
        }

        var filesData = new FilesData(fileContents, BucketName);
        
        var uploadResult = await _fileProvider.UploadFilesAsync(filesData, cancellationToken);
        if (uploadResult.IsFailure)
            return uploadResult.Error;

        var photoPaths = command.Files
            .Select(f => PhotoPath.Create(Guid.NewGuid(), f.FileName).Value);

        var petPhotos = photoPaths.Select(f => new Photo(f));
        
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
            new Photos(petPhotos));

        volunteerResult.Value.AddPet(pet);
        
        await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);

        return pet.Id.Value;
    }
} 