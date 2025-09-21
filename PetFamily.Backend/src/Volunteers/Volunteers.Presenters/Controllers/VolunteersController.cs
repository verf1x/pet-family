using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Core.Abstractions;
using PetFamily.Framework;
using PetFamily.Framework.Processors;
using PetFamily.Framework.ResponseExtensions;
using PetFamily.SharedKernel.Models;
using Volunteers.Application.VolunteersManagement.Queries.GetById;
using Volunteers.Application.VolunteersManagement.Queries.GetWithPagination;
using Volunteers.Application.VolunteersManagement.UseCases.AddPet;
using Volunteers.Application.VolunteersManagement.UseCases.Create;
using Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;
using Volunteers.Application.VolunteersManagement.UseCases.Delete.Soft;
using Volunteers.Application.VolunteersManagement.UseCases.HardDeletePet;
using Volunteers.Application.VolunteersManagement.UseCases.MovePet;
using Volunteers.Application.VolunteersManagement.UseCases.RemovePetPhotos;
using Volunteers.Application.VolunteersManagement.UseCases.SetMainPetPhoto;
using Volunteers.Application.VolunteersManagement.UseCases.SoftDeletePet;
using Volunteers.Application.VolunteersManagement.UseCases.UpdateMainInfo;
using Volunteers.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;
using Volunteers.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;
using Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;
using Volunteers.Contracts.Dtos.Volunteer;
using Volunteers.Contracts.Requests.Volunteers;

namespace Volunteers.Presenters.Controllers;

[Authorize]
public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync(
        [FromServices] ICommandHandler<Guid, CreateVolunteerCommand> handler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateVolunteerCommand command = new(
            request.FullName,
            request.Email,
            request.Description,
            request.ExperienceYears,
            request.PhoneNumber,
            request.SocialNetworks,
            request.HelpRequisites);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPut("{id:guid}/main-info")]
    public async Task<IActionResult> UpdateMainInfoAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoRequest request,
        [FromServices] ICommandHandler<Guid, UpdateMainInfoCommand> handler,
        CancellationToken cancellationToken = default)
    {
        UpdateMainInfoCommand command = new(
            id,
            request.FullName,
            request.Email,
            request.Description,
            request.ExperienceYears,
            request.PhoneNumber);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{id:guid}/soft")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<Guid, SoftDeleteVolunteerCommand> handler,
        CancellationToken cancellationToken = default)
    {
        SoftDeleteVolunteerCommand command = new(id);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{id:guid}/hard")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<Guid, HardDeleteVolunteerCommand> handler,
        CancellationToken cancellationToken = default)
    {
        HardDeleteVolunteerCommand command = new(id);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPost("{id:guid}/pet")]
    public async Task<IActionResult> AddPetAsync(
        [FromRoute] Guid id,
        [FromForm] AddPetRequest request,
        [FromServices] ICommandHandler<Guid, AddPetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        AddPetCommand command = new(
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
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
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
        await using FormFileProcessor fileProcessor = new();

        var fileDtos = fileProcessor.Process(photos);

        UploadPetPhotosCommand command = new(volunteerId, petId, fileDtos);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> RemovePetPhotosAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromQuery] IEnumerable<string> photoPaths,
        [FromServices] ICommandHandler<List<string>, RemovePetPhotosCommand> handler,
        CancellationToken cancellationToken = default)
    {
        RemovePetPhotosCommand command = new(volunteerId, petId, photoPaths);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/pet/{petId:guid}/move/{newPosition:int}")]
    public async Task<IActionResult> MovePetAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromRoute] int newPosition,
        [FromServices] ICommandHandler<int, MovePetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        MovePetCommand command = new(volunteerId, petId, newPosition);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetVolunteersWithPaginationRequest request,
        [FromServices] IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery> handler,
        CancellationToken cancellationToken = default)
    {
        GetVolunteersWithPaginationQuery query = new(
            request.PageNumber,
            request.PageSize);

        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<VolunteerDto, GetVolunteerByIdQuery> handler,
        CancellationToken cancellationToken = default)
    {
        GetVolunteerByIdQuery query = new(id);

        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/pet/{petId:guid}/main-info")]
    public async Task<IActionResult> UpdateMainPetInfoAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] UpdateMainPetInfoRequest request,
        [FromServices] ICommandHandler<Guid, UpdateMainPetInfoCommand> handler,
        CancellationToken cancellationToken = default)
    {
        UpdateMainPetInfoCommand command = new(
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
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/pet/{petId:guid}/help-status")]
    public async Task<IActionResult> UpdatePetHelpStatusAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] int helpStatus,
        [FromServices] ICommandHandler<int, UpdatePetHelpStatusCommand> handler,
        CancellationToken cancellationToken = default)
    {
        UpdatePetHelpStatusCommand command = new(volunteerId, petId, helpStatus);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/soft")]
    public async Task<IActionResult> DeletePetAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] ICommandHandler<Guid, SoftDeletePetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        SoftDeletePetCommand command = new(volunteerId, petId);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/hard")]
    public async Task<IActionResult> DeletePetAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] ICommandHandler<Guid, HardDeletePetCommand> handler,
        CancellationToken cancellationToken = default)
    {
        HardDeletePetCommand command = new(volunteerId, petId);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPut("{volunteerId}/pet/{petId:guid}/main-photo")]
    public async Task<IActionResult> UpdateMainPetPhotoAsync(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] string photoPath,
        [FromServices] ICommandHandler<string, SetMainPetPhotoCommand> handler,
        CancellationToken cancellationToken = default)
    {
        SetMainPetPhotoCommand command = new(volunteerId, petId, photoPath);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}