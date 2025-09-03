using PetFamily.Framework.Abstractions;
using Volunteers.Contracts.Dtos;
using Volunteers.Contracts.Dtos.Pet;

namespace Volunteers.Application.VolunteersManagement.UseCases.AddPet;

public record AddPetCommand(
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
    int HelpStatus,
    IEnumerable<HelpRequisiteDto> HelpRequisites) : ICommand;