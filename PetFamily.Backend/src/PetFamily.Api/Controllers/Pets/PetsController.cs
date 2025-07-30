using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Controllers.Pets.Requests;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;
using PetFamily.Application.Models;
using PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

namespace PetFamily.Api.Controllers.Pets;

public class PetsController : ApplicationController
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetFilteredPetsWithPaginationRequest request,
        [FromServices] IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery> handler,
        CancellationToken cancellationToken = default)
    {
        var query = request.ToQuery();
        
        var response = await handler.HandleAsync(query, cancellationToken);

        return Ok(response); 
    }
}