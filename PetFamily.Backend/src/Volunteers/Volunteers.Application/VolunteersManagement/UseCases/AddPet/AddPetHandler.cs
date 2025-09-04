using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;
using Volunteers.Application.Extensions;

namespace Volunteers.Application.VolunteersManagement.UseCases.AddPet;

public class AddPetHandler : ICommandHandler<Guid, AddPetCommand>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public AddPetHandler(
        IVolunteersRepository volunteersRepository,
        IValidator<AddPetCommand> validator,
        IUnitOfWork unitOfWork,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        Result<Volunteer, Error> volunteerResult = await _volunteersRepository
            .GetByIdAsync(VolunteerId.Create(command.VolunteerId), cancellationToken);

        if (volunteerResult.IsFailure)
        {
            return volunteerResult.Error.ToErrorList();
        }

        Result<Pet, ErrorList> petResult = await InitializePetAsync(command);
        if (petResult.IsFailure)
        {
            return petResult.Error;
        }

        Pet? pet = petResult.Value;

        volunteerResult.Value.AddPet(pet);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return pet.Id.Value;
    }

    private async Task<Result<Pet, ErrorList>> InitializePetAsync(AddPetCommand command)
    {
        PetId petId = PetId.CreateNew();
        Nickname? nickname = Nickname.Create(command.Nickname).Value;
        Description? description = Description.Create(command.Description).Value;

        SpeciesId speciesId = SpeciesId.Create(command.SpeciesBreedDto.SpeciesId);
        if (!await speciesId.IsSpeciesExistsAsync(_sqlConnectionFactory))
        {
            return Errors.General.ValueIsInvalid(nameof(command.SpeciesBreedDto.SpeciesId)).ToErrorList();
        }

        BreedId breedId = BreedId.Create(command.SpeciesBreedDto.BreedId);
        if (!await breedId.IsBreedExistsAsync(_sqlConnectionFactory))
        {
            return Errors.General.ValueIsInvalid(nameof(command.SpeciesBreedDto.BreedId)).ToErrorList();
        }

        SpeciesBreed? speciesBreed = SpeciesBreed.Create(speciesId, breedId).Value;

        Color? color = Color.Create(command.Color).Value;
        HealthInfo? healthInfo = HealthInfo.Create(
            command.HealthInfoDto.HealthStatus,
            command.HealthInfoDto.IsNeutered,
            command.HealthInfoDto.IsVaccinated).Value;

        Address? address = Address.Create(
            command.AddressDto.AddressLines.ToList(),
            command.AddressDto.Locality,
            command.AddressDto.Region,
            command.AddressDto.PostalCode,
            command.AddressDto.CountryCode).Value;

        Measurements? measurements = Measurements.Create(
            command.MeasurementsDto.Height,
            command.MeasurementsDto.Weight).Value;

        PhoneNumber? ownerPhoneNumber = PhoneNumber.Create(command.OwnerPhoneNumber).Value;
        DateOnly dateOfBirth = command.DateOfBirth;
        HelpStatus helpStatus = (HelpStatus)command.HelpStatus;

        List<HelpRequisite> helpRequisites = command.HelpRequisites
            .Select(r => HelpRequisite.Create(r.Name, r.Description).Value)
            .ToList();

        Pet pet = new(
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
            helpRequisites);

        return pet;
    }
}