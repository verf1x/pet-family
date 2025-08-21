using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Domain.VolunteersManagement.Enums;

namespace PetFamily.Contracts.Requests.Volunteers;

public record UpdateMainPetInfoRequest(
    string Nickname,
    string Description,
    SpeciesBreedDto SpeciesBreed,
    string Color,
    HealthInfoDto HealthInfo,
    AddressDto Address,
    MeasurementsDto Measurements,
    string OwnerPhoneNumber,
    DateOnly DateOfBirth,
    List<HelpRequisiteDto> HelpRequisites);