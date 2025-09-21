using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Core.Abstractions;
using PetFamily.Framework;
using PetFamily.Framework.ResponseExtensions;
using PetFamily.SharedKernel;
using Species.Application.SpeciesManagement.Queries.Get;
using Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;
using Species.Application.SpeciesManagement.UseCases.AddBreeds;
using Species.Application.SpeciesManagement.UseCases.Create;
using Species.Application.SpeciesManagement.UseCases.Delete;
using Species.Application.SpeciesManagement.UseCases.DeleteBreed;
using Species.Contracts.Dtos.Species;

namespace Species.Presenters;

[Authorize]
public class SpeciesController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromServices] ICommandHandler<Guid, CreateSpeciesCommand> handler,
        [FromBody] string name,
        CancellationToken cancellationToken = default)
    {
        CreateSpeciesCommand command = new(name);

        Result<Guid, ErrorList> result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPost("{id:guid}/breeds")]
    public async Task<IActionResult> AddBreedsAsync(
        [FromServices] ICommandHandler<List<Guid>, AddBreedsCommand> handler,
        [FromRoute] Guid id,
        [FromBody] IEnumerable<string> breedsNames)
    {
        AddBreedsCommand command = new(id, breedsNames);

        Result<List<Guid>, ErrorList> result = await handler.HandleAsync(command);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ICommandHandler<Guid, DeleteSpeciesCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        DeleteSpeciesCommand command = new(id);

        Result<Guid, ErrorList> result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{speciesId:guid}/breeds/{breedId:guid}")]
    public async Task<IActionResult> DeleteBreedAsync(
        [FromServices] ICommandHandler<Guid, DeleteBreedCommand> handler,
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        CancellationToken cancellationToken = default)
    {
        DeleteBreedCommand command = new(speciesId, breedId);

        Result<Guid, ErrorList> result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromServices] IQueryHandler<IReadOnlyList<SpeciesDto>, GetAllSpeciesQuery> handler,
        CancellationToken cancellationToken = default)
    {
        Result<IReadOnlyList<SpeciesDto>, ErrorList> result =
            await handler.HandleAsync(new GetAllSpeciesQuery(),
                cancellationToken); //TODO: убрать пустой GetAllSpeciesQuery
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}/breeds")]
    public async Task<IActionResult> GetBreedsAsync(
        [FromServices] IQueryHandler<IReadOnlyList<BreedDto>, GetBreedsBySpeciesIdQuery> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        GetBreedsBySpeciesIdQuery query = new(id);

        Result<IReadOnlyList<BreedDto>, ErrorList> result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}