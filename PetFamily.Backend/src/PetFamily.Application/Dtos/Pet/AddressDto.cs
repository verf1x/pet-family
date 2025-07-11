namespace PetFamily.Application.Volunteers.AddPet;

public record AddressDto(
    IEnumerable<string> AddressLines,
    string Locality,
    string? Region,
    string? PostalCode,
    string CountryCode);