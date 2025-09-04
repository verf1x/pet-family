using Volunteers.Contracts.Dtos;
using Volunteers.Contracts.Dtos.Pet;

namespace Volunteers.Contracts.Requests.Volunteers;

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