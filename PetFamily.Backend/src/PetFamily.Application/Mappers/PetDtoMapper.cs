using System.Text.Json;
using PetFamily.Contracts.Dtos.Pet;

namespace PetFamily.Application.Mappers;

public static class PetDtoMapper
{
    public static Func<PetDtoFlat, string, PetDto> MapFlatDtoToPetDtoWithPhotos() =>
        (petFlat, jsonFiles) => new PetDto
        {
            Id = petFlat.Id,
            VolunteerId = petFlat.VolunteerId,
            Nickname = petFlat.Nickname,
            BirthDate = DateOnly.FromDateTime(petFlat.BirthDate),
            Description = petFlat.Description,
            Position = petFlat.Position,
            Color = petFlat.Color,
            HelpStatus = petFlat.HelpStatus,
            SpeciesBreed = new SpeciesBreedDto(petFlat.SpeciesId, petFlat.BreedId),
            Measurements = new MeasurementsDto(petFlat.Height, petFlat.Weight),
            Address = new AddressDto(
                JsonSerializer.Deserialize<IEnumerable<string>>(petFlat.AddressLines) ?? [],
                petFlat.Locality,
                petFlat.Region,
                petFlat.PostalCode,
                petFlat.CountryCode),
            Photos = !string.IsNullOrWhiteSpace(jsonFiles)
                ? JsonSerializer.Deserialize<PetFileDto[]>(jsonFiles)!
                : [],
        };
}