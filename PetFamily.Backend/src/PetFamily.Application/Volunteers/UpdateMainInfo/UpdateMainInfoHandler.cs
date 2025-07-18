using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IValidator<UpdateMainInfoCommand> _validator;
    private readonly ILogger<UpdateMainInfoHandler> _logger;

    public UpdateMainInfoHandler(
        IVolunteersRepository volunteersRepository,
        IApplicationDbContext dbContext,
        IValidator<UpdateMainInfoCommand> validator,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateMainInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var volunteerId = VolunteerId.Create(command.VolunteerId);
        
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();
        
        var fullName = FullName.Create(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.MiddleName).Value;
        
        var email = Email.Create(command.Email).Value;
        var description = Description.Create(command.Description).Value;
        var experienceYears = Experience.Create(command.ExperienceYears).Value;
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;
        
        volunteerResult.Value.UpdateMainInfo(
            fullName,
            email,
            description,
            experienceYears,
            phoneNumber);
        
        _dbContext.Volunteers.Attach(volunteerResult.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Updated volunteer with ID: {VolunteerId}", command.VolunteerId); 
        
        return volunteerId.Value;
    }
}