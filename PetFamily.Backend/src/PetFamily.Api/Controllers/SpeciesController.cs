using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Application.Abstractions;
using PetFamily.Application.SpeciesManagement.UseCases.AddBreeds;
using PetFamily.Application.SpeciesManagement.UseCases.Create;
using PetFamily.Application.SpeciesManagement.UseCases.Delete;
using PetFamily.Application.SpeciesManagement.UseCases.DeleteBreed;

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
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ICommandHandler<Guid, DeleteSpeciesCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSpeciesCommand(id);
        
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        return Ok(result.Value);
    }
    
    [HttpDelete("{speciesId:guid}/breeds/{breedId:guid}")]
    public async Task<IActionResult> DeleteBreedAsync(
        [FromServices] ICommandHandler<Guid, DeleteBreedCommand> handler,
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBreedCommand(speciesId, breedId);
        
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        return Ok(result.Value);
    }
}