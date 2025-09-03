using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Abstractions;
using PetFamily.Framework.Models;
using PetFamily.Framework.ResponseExtensions;
using Volunteers.Application.VolunteersManagement.Queries.GetFilteredPetsWithPagination;
using Volunteers.Application.VolunteersManagement.Queries.GetPetById;
using Volunteers.Contracts.Dtos.Pet;
using Volunteers.Contracts.Requests.Pets;

namespace Volunteers.Presenters.Controllers;

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
        [FromServices] IQueryHandler<PetDto, GetPetByIdQuery> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPetByIdQuery(petId);

        var result = await handler.HandleAsync(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}