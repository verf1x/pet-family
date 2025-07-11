using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Contracts;
using PetFamily.Api.Extensions;
using PetFamily.Application.Dtos.Pet;
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
        [FromBody] CreateVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/main-info")]
    public async Task<IActionResult> UpdateMainInfoAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoDto dto,
        [FromServices] UpdateMainInfoHandler handler,
        [FromServices] IValidator<UpdateMainInfoCommand> validator,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMainInfoCommand(id, dto);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.ToValidationErrorResponse();

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}/soft")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromServices] IValidator<DeleteVolunteerCommand> validator,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteVolunteerCommand(id);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.ToValidationErrorResponse();

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}/hard")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] HardDeleteVolunteerHandler handler,
        [FromServices] IValidator<DeleteVolunteerCommand> validator,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteVolunteerCommand(id);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid is false)
            return validationResult.ToValidationErrorResponse();

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/pet")]
    public async Task<IActionResult> AddPet(
        [FromRoute] Guid id,
        [FromForm] AddPetRequest request,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken = default)
    {
        var filesDto = new List<FileDto>();

        try
        {
            filesDto.AddRange(
                from file in request.Files
                let stream = file.OpenReadStream()
                select new FileDto(stream, file.FileName));
            
            var command = new AddPetCommand(
                id,
                request.Nickname,
                request.Description,
                // request.SpeciesBreedDto, //TODO
                request.Color,
                request.HealthInfoDto,
                request.AddressDto,
                request.MeasurementsDto,
                request.OwnerPhoneNumber,
                request.DateOfBirth,
                request.HelpStatus,
                request.HelpRequisites,
                filesDto);

            var result = await handler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }
        finally
        {
            foreach (var fileDto in filesDto)
                await fileDto.Content.DisposeAsync();
        }
    }
}