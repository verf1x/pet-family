using Microsoft.AspNetCore.Mvc;
using PetFamily.Application.VolunteersManagement.Queries.GetFilteredPetsWithPagination;
using PetFamily.Contracts.Requests.Pets;

namespace PetFamily.Api.Controllers;

public class PetsController : ApplicationController
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetFilteredPetsWithPaginationRequest request,
        [FromServices] GetFilteredPetsWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        
        var query = new GetFilteredPetsWithPaginationQuery(
            request.Nickname,
            request.PositionFrom,
            request.PositionTo,
            request.SortBy,
            request.SortAscending,
            request.PageNumber,
            request.PageSize);
        
        var response = await handler.HandleAsync(query, cancellationToken);

        return Ok(response); 
    }
    
    [HttpGet("dapper")]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetFilteredPetsWithPaginationRequest request,
        [FromServices] GetFilteredPetsWithPaginationHandlerDapper handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFilteredPetsWithPaginationQuery(
            request.Nickname,
            request.PositionFrom,
            request.PositionTo,
            request.SortBy,
            request.SortAscending,
            request.PageNumber,
            request.PageSize);
        
        var response = await handler.HandleAsync(query, cancellationToken);

        return Ok(response); 
    }
}