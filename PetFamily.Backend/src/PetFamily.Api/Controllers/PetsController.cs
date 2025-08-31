using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Models;
using PetFamily.Application.VolunteersManagement.Queries.GetFilteredPetsWithPagination;
using PetFamily.Application.VolunteersManagement.Queries.GetPetById;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Contracts.Requests.Pets;

namespace PetFamily.Api.Controllers;

public class PetsController : ApplicationController
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetFilteredPetsWithPaginationRequest request,
        [FromServices] IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFilteredPetsWithPaginationQuery(
            request.VolunteerIds,
            request.Nickname,
            request.Age,
            request.SpeciesId,
            request.BreedId,
            request.Color,
            request.CountryCode,
            request.Locality,
            request.Height,
            request.Weight,
            request.HelpStatus,
            request.PositionFrom,
            request.PositionTo,
            request.SortBy,
            request.SortAscending,
            request.PageNumber,
            request.PageSize);

        var result = await handler.HandleAsync(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpGet("{petId:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        Guid petId,
        [FromServices] GetPetByIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPetByIdQuery(petId);

        var result = await handler.HandleAsync(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}