using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;

namespace PetFamily.Application.Volunteers.Delete;

public class SoftDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersesRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger;

    public SoftDeleteVolunteerHandler(
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
        
        volunteerResult.Value.SoftDelete();
        
        var result = await _volunteersesRepository.SaveAsync(volunteerResult.Value, cancellationToken);
        
        _logger.LogInformation("Soft deleted volunteer with ID: {VolunteerId}", command.VolunteerId); 
        
        return result;
    }
}