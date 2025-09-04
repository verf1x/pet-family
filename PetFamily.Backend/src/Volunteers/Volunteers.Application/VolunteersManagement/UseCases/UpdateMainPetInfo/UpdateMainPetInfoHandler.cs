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
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;
using Volunteers.Application.Extensions;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;

public class UpdateMainPetInfoHandler : ICommandHandler<Guid, UpdateMainPetInfoCommand>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMainPetInfoCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public UpdateMainPetInfoHandler(
        IValidator<UpdateMainPetInfoCommand> validator,
        IVolunteersRepository volunteersRepository,
        IUnitOfWork unitOfWork,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateMainPetInfoCommand command,
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

        Nickname? nickname = Nickname.Create(command.Nickname).Value;
        Description? description = Description.Create(command.Description).Value;

        SpeciesId speciesId = SpeciesId.Create(command.SpeciesBreed.SpeciesId);
        if (!await speciesId.IsSpeciesExistsAsync(_sqlConnectionFactory))
        {
            return Errors.General.ValueIsInvalid(nameof(command.SpeciesBreed.SpeciesId)).ToErrorList();
        }

        BreedId breedId = BreedId.Create(command.SpeciesBreed.BreedId);
        if (!await breedId.IsBreedExistsAsync(_sqlConnectionFactory))
        {
            return Errors.General.ValueIsInvalid(nameof(command.SpeciesBreed.BreedId)).ToErrorList();
        }

        SpeciesBreed? speciesBreed = SpeciesBreed.Create(speciesId, breedId).Value;

        Color? color = Color.Create(command.Color).Value;

        HealthInfo? healthInfo = HealthInfo.Create(
            command.HealthInfo.HealthStatus,
            command.HealthInfo.IsNeutered,
            command.HealthInfo.IsVaccinated).Value;

        Address? address = Address.Create(
            command.Address.AddressLines.ToList(),
            command.Address.Locality,
            command.Address.Region,
            command.Address.PostalCode,
            command.Address.CountryCode).Value;

        Measurements? measurements = Measurements.Create(
            command.Measurements.Height,
            command.Measurements.Weight).Value;

        PhoneNumber? ownerPhoneNumber = PhoneNumber.Create(command.OwnerPhoneNumber).Value;

        List<HelpRequisite> helpRequisites = command.HelpRequisites
            .Select(r => HelpRequisite.Create(r.Name, r.Description).Value)
            .ToList();

        petResult.Value.UpdateMainInfo(
            nickname,
            description,
            speciesBreed,
            color,
            healthInfo,
            address,
            measurements,
            ownerPhoneNumber,
            command.DateOfBirth,
            helpRequisites);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return petId.Value;
    }
}