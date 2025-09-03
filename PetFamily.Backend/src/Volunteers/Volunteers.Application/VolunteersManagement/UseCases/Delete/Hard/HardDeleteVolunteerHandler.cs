using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Framework;
using PetFamily.Framework.Abstractions;
using PetFamily.Framework.EntityIds;
using PetFamily.Framework.Extensions;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

public class HardDeleteVolunteerHandler : ICommandHandler<Guid, HardDeleteVolunteerCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IValidator<HardDeleteVolunteerCommand> _validator;
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;

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
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var result = await _volunteersRepository.RemoveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation("Hard deleted volunteer with ID: {VolunteerId}", command.VolunteerId);

        return result;
    }
}