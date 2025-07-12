using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;

namespace PetFamily.Application.Volunteers.Delete;

public class HardDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersesRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger;

    public HardDeleteVolunteerHandler(
        IVolunteersRepository volunteersesRepository,
        ILogger<SoftDeleteVolunteerHandler> logger)
    {
        _volunteersesRepository = volunteersesRepository;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Error>> HandleAsync(
        DeleteVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersesRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;
        
        var result = await _volunteersesRepository.DeleteAsync(volunteerResult.Value, cancellationToken);
        
        _logger.LogInformation("Hard deleted volunteer with ID: {VolunteerId}", command.VolunteerId); 
        
        return result;
    }
}