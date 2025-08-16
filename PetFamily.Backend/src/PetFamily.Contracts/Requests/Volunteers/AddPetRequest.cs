using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Domain.VolunteersManagement.Enums; // TODO: придумать как убрать зависимость от Domain

namespace PetFamily.Contracts.Requests.Volunteers;

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
    HelpStatus HelpStatus,
    IEnumerable<HelpRequisiteDto> HelpRequisites);