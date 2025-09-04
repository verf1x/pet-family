namespace Volunteers.Contracts.Dtos.Pet;

public record AddressDto(
    IEnumerable<string> AddressLines,
    string Locality,
    string? Region,
    string? PostalCode,
    string CountryCode);