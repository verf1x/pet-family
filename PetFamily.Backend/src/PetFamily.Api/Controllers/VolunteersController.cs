using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Api.Processors;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Models;
using PetFamily.Application.VolunteersManagement.Queries.GetById;
using PetFamily.Application.VolunteersManagement.Queries.GetWithPagination;
using PetFamily.Application.VolunteersManagement.UseCases.AddPet;
using PetFamily.Application.VolunteersManagement.UseCases.Create;
using PetFamily.Application.VolunteersManagement.UseCases.Delete;
using PetFamily.Application.VolunteersManagement.UseCases.HardDeletePet;
using PetFamily.Application.VolunteersManagement.UseCases.MovePet;
using PetFamily.Application.VolunteersManagement.UseCases.RemovePetPhotos;
using PetFamily.Application.VolunteersManagement.UseCases.SetMainPetPhoto;
using PetFamily.Application.VolunteersManagement.UseCases.SoftDeletePet;
using PetFamily.Application.VolunteersManagement.UseCases.UpdateMainInfo;
using PetFamily.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;
using PetFamily.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;
using PetFamily.Application.VolunteersManagement.UseCases.UploadPetPhotos;
using PetFamily.Contracts.Dtos.Volunteer;
using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Api.Controllers;

public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync(
        [FromServices] ICommandHandler<Guid, CreateVolunteerCommand> handler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateVolunteerCommand(
            request.FullName,
            request.Email,
            request.Description,
            request.ExperienceYears,
            request.PhoneNumber,
            request.SocialNetworks,
            request.HelpRequisites);

        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/main-info")]
    public async Task<IActionResult> UpdateMainInfoAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoRequest request,
        [FromServices] ICommandHandler<Guid, UpdateMainInfoCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMainInfoCommand(
            id,
            request.FullName,
            request.Email,
            request.Description,
            request.ExperienceYears,
            request.PhoneNumber);

        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}/soft")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] SoftDeleteVolunteerHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteVolunteerCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}/hard")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] HardDeleteVolunteerHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteVolunteerCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/pet")]
    public async Task<IActionResult> AddPetAsync(
        [FromRoute] Guid id,
        [FromForm] AddPetRequest request,
        [FromServices] ICommandHandler<Guid, AddPetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddPetCommand(
            id,
            request.Nickname,
            request.Description,
            request.SpeciesBreedDto,
            request.Color,
            request.HealthInfoDto,
            request.AddressDto,
            request.MeasurementsDto,
            request.OwnerPhoneNumber,
            request.DateOfBirth,
            request.HelpStatus,
            request.HelpRequisites);

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{volunteerId:guid}/pet/{petId:guid}/photos")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPetPhotosAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] IFormFileCollection photos,
        [FromServices] ICommandHandler<List<string>, UploadPetPhotosCommand> handler,
        CancellationToken cancellationToken = default)
    {
        await using var fileProcessor = new FormFileProcessor();

        var fileDtos = fileProcessor.Process(photos);

        var command = new UploadPetPhotosCommand(volunteerId, petId, fileDtos);

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> RemovePetPhotosAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromQuery] IEnumerable<string> photoPaths,
        [FromServices] ICommandHandler<List<string>, RemovePetPhotosCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemovePetPhotosCommand(volunteerId, petId, photoPaths);

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/pet/{petId:guid}/move/{newPosition:int}")]
    public async Task<IActionResult> MovePetAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromRoute] int newPosition,
        [FromServices] ICommandHandler<int, MovePetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new MovePetCommand(volunteerId, petId, newPosition);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetVolunteersWithPaginationRequest request,
        [FromServices] IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetVolunteersWithPaginationQuery(
            request.PageNumber,
            request.PageSize);

        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<VolunteerDto, GetVolunteerByIdQuery> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetVolunteerByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/pet/{petId:guid}/main-info")]
    public async Task<IActionResult> UpdateMainPetInfoAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] UpdateMainPetInfoRequest request,
        [FromServices] ICommandHandler<Guid, UpdateMainPetInfoCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMainPetInfoCommand(
            volunteerId,
            petId,
            request.Nickname,
            request.Description,
            request.SpeciesBreed,
            request.Color,
            request.HealthInfo,
            request.Address,
            request.Measurements,
            request.OwnerPhoneNumber,
            request.DateOfBirth,
            request.HelpRequisites);

        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/pet/{petId:guid}/help-status")]
    public async Task<IActionResult> UpdatePetHelpStatusAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] int helpStatus,
        [FromServices] ICommandHandler<int, UpdatePetHelpStatusCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdatePetHelpStatusCommand(volunteerId, petId, helpStatus);

        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/soft")]
    public async Task<IActionResult> DeletePetAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] ICommandHandler<Guid, SoftDeletePetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new SoftDeletePetCommand(volunteerId, petId);

        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/hard")]
    public async Task<IActionResult> DeletePetAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] ICommandHandler<Guid, HardDeletePetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new HardDeletePetCommand(volunteerId, petId);

        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{volunteerId}/pet/{petId:guid}/main-photo")]
    public async Task<IActionResult> UpdateMainPetPhotoAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] string photoPath,
        [FromServices] ICommandHandler<string, SetMainPetPhotoCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new SetMainPetPhotoCommand(volunteerId, petId, photoPath);

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}