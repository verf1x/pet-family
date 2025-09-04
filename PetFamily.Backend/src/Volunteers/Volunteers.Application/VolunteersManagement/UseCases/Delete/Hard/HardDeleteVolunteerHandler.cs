using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

public class HardDeleteVolunteerHandler : ICommandHandler<Guid, HardDeleteVolunteerCommand>
{
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;
    private readonly IValidator<HardDeleteVolunteerCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public HardDeleteVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        IValidator<HardDeleteVolunteerCommand> validator,
        ILogger<HardDeleteVolunteerHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        HardDeleteVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId volunteerId = VolunteerId.Create(command.VolunteerId);
        Result<Volunteer, Error> volunteerResult =
            await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            return volunteerResult.Error.ToErrorList();
        }

        Guid result = await _volunteersRepository.RemoveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation("Hard deleted volunteer with ID: {VolunteerId}", command.VolunteerId);

        return result;
    }
}