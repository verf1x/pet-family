using PetFamily.Application.Abstractions;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Domain.VolunteersManagement.Enums;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;

public record UpdateMainPetInfoCommand(
    Guid VolunteerId,
    Guid PetId,
    string Nickname,
    string Description,
    SpeciesBreedDto SpeciesBreed,
    string Color,
    HealthInfoDto HealthInfo,
    AddressDto Address,
    MeasurementsDto Measurements,
    string OwnerPhoneNumber,
    DateOnly DateOfBirth,
    List<HelpRequisiteDto> HelpRequisites) : ICommand;