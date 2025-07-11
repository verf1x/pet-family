using PetFamily.Application.Dtos;
using PetFamily.Application.Volunteers.AddPet; //TODO: мб нужны dto для api слоя, иначе архитектура ломается
using PetFamily.Domain.Volunteers.Enums;

namespace PetFamily.Api.Contracts;

public record AddPetRequest(
    Guid VolunteerId,
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
    IEnumerable<HelpRequisiteDto> HelpRequisites,
    IFormFileCollection Files);