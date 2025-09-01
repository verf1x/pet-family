namespace PetFamily.Contracts.Requests.Pets;

public record GetFilteredPetsWithPaginationRequest(
    Guid[]? VolunteerIds,
    string? Nickname,
    int? Age,
    Guid? SpeciesId,
    Guid? BreedId,
    string? Color,
    string? CountryCode,
    string? Locality,
    float? Height,
    float? Weight,
    int? HelpStatus,
    int? PositionFrom,
    int? PositionTo,
    string? SortBy,
    bool? SortAscending,
    int PageNumber,
    int PageSize);