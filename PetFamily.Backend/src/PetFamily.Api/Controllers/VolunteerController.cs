using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Application.Volunteers.CreateVolunteer;

namespace PetFamily.Api.Controllers;

public class VolunteerController : ApplicationController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromServices] IValidator<CreateVolunteerRequest> validator,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();
        
        var result = await handler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return CreatedAtAction("", result.Value);
    }
}