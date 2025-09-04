namespace Volunteers.Contracts.Requests.Volunteers;

public record GetVolunteersWithPaginationRequest(int PageNumber, int PageSize);