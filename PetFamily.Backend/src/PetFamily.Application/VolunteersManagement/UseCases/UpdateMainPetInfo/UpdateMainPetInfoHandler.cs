using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;

public class UpdateMainPetInfoHandler : ICommandHandler<Guid, UpdateMainPetInfoCommand>
{
    private readonly IValidator<UpdateMainPetInfoCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

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

        var nickname = Nickname.Create(command.Nickname).Value;
        var description = Description.Create(command.Description).Value;

        var speciesId = SpeciesId.Create(command.SpeciesBreed.SpeciesId);
        if (!(await speciesId.IsSpeciesExistsAsync(_sqlConnectionFactory)))
            return Errors.General.ValueIsInvalid(nameof(command.SpeciesBreed.SpeciesId)).ToErrorList();

        var breedId = BreedId.Create(command.SpeciesBreed.BreedId);
        if (!(await breedId.IsBreedExistsAsync(_sqlConnectionFactory)))
            return Errors.General.ValueIsInvalid(nameof(command.SpeciesBreed.BreedId)).ToErrorList();

        var speciesBreed = SpeciesBreed.Create(speciesId, breedId).Value;

        var color = Color.Create(command.Color).Value;

        var healthInfo = HealthInfo.Create(
            command.HealthInfo.HealthStatus,
            command.HealthInfo.IsNeutered,
            command.HealthInfo.IsVaccinated).Value;

        var address = Address.Create(
            command.Address.AddressLines.ToList(),
            command.Address.Locality,
            command.Address.Region,
            command.Address.PostalCode,
            command.Address.CountryCode).Value;

        var measurements = Measurements.Create(
            command.Measurements.Height,
            command.Measurements.Weight).Value;

        var ownerPhoneNumber = PhoneNumber.Create(command.OwnerPhoneNumber).Value;

        var helpRequisites = command.HelpRequisites
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