namespace PetFamily.Contracts.Requests.Pets;

public record GetFilteredPetsWithPaginationRequest(
    string? Nickname,
    int? PositionFrom,
    int? PositionTo,
    string? SortBy,
    bool? SortAscending,
    int PageNumber,
    int PageSize);