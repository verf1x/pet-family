using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.Create;
using PetFamily.Application.Volunteers.Delete;
using PetFamily.Application.Volunteers.UpdateMainInfo;

namespace PetFamily.Api.Controllers;

public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync(
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
    
    [HttpPut("{id:guid}/main-info")]
    public async Task<IActionResult> UpdateMainInfoAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoDto dto,
        [FromServices] UpdateMainInfoHandler handler,
        [FromServices] IValidator<UpdateMainInfoRequest> validator,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateMainInfoRequest(id, dto);
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.ToValidationErrorResponse();
        
        var result = await handler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
    
    [HttpDelete("{id:guid}/soft")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromServices] IValidator<DeleteVolunteerRequest> validator,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteVolunteerRequest(id);
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.ToValidationErrorResponse();
        
        var result = await handler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
    
    [HttpDelete("{id:guid}/hard")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] HardDeleteVolunteerHandler handler,
        [FromServices] IValidator<DeleteVolunteerRequest> validator,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteVolunteerRequest(id);
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.ToValidationErrorResponse();
        
        var result = await handler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("pet")]
    public async Task<IActionResult> AddPet(
        IFormFile file,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken = default)
    {
        await using var stream = file.OpenReadStream();

        var path = Guid.NewGuid().ToString();

        var fileData = new FileData(stream, "photos", path);

        var result = await handler.HandleAsync(fileData, cancellationToken);
        if(result.IsFailure)
            return result.Error.ToResponse();
        
        return Ok(result.Value);
    }
} 