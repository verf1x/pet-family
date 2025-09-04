using Volunteers.Contracts.Dtos;
using Volunteers.Contracts.Dtos.Pet;

namespace Volunteers.Contracts.Requests.Volunteers;

public record AddPetRequest(
    string Nickname,
    string Description,
    SpeciesBreedDto SpeciesBreedDto,
    string Color,
    HealthInfoDto HealthInfoDto,
    AddressDto AddressDto,
    MeasurementsDto MeasurementsDto,
    string OwnerPhoneNumber,
    DateOnly DateOfBirth,
    int HelpStatus,
    IEnumerable<HelpRequisiteDto> HelpRequisites);