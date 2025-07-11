using PetFamily.Application.Dtos;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Domain.Volunteers.Enums;
//TODO: мб нужны dto для api слоя, иначе архитектура ломается

namespace PetFamily.Api.Controllers.Volunteers;

public record AddPetRequest(
    string Nickname,
    string Description,
    // SpeciesBreedDto SpeciesBreedDto,
    string Color,
    HealthInfoDto HealthInfoDto,
    AddressDto AddressDto,
    MeasurementsDto MeasurementsDto,
    string OwnerPhoneNumber,
    DateOnly DateOfBirth,
    HelpStatus HelpStatus,
    IEnumerable<HelpRequisiteDto> HelpRequisites,
    IFormFileCollection Files);