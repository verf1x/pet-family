using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Application.Abstractions;
using PetFamily.Application.SpeciesManagement.UseCases.AddBreeds;
using PetFamily.Application.SpeciesManagement.UseCases.Create;

namespace PetFamily.Api.Controllers;

public class SpeciesController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromServices] ICommandHandler<Guid, CreateSpeciesCommand> handler,
        [FromBody] string name,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateSpeciesCommand(name);
        
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/breeds")]
    public async Task<IActionResult> AddBreedsAsync(
        [FromServices] ICommandHandler<List<Guid>, AddBreedsCommand> handler,
        [FromRoute] Guid id,
        [FromBody] IEnumerable<string> breedsNames)
    {
        var command = new AddBreedsCommand(id, breedsNames);
        
        var result = await handler.HandleAsync(command);
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        return Ok(result.Value);
    }
}