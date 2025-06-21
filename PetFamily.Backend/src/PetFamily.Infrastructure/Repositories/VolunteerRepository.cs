using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.Entities;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteerRepository : IVolunteerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VolunteerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default)
    {
        await _dbContext.Volunteers.AddAsync(volunteer, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return volunteer.Id;
    }

    public async Task<Result<Volunteer, Error>> GetByIdAsync(
        VolunteerId volunteerId,
        CancellationToken cancellationToken = default)
    {
        var volunteer = await _dbContext.Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == volunteerId, cancellationToken: cancellationToken);

        if (volunteer is null)
            return Errors.General.NotFound(volunteerId.Value);

        return volunteer;
    }

    public async Task<Result<Volunteer>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var volunteer = await _dbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Email == email, cancellationToken: cancellationToken);
        
        return volunteer ?? Result.Failure<Volunteer>("Email number not found");
    }

    public async Task<Result<Volunteer>> GetByPhoneNumberAsync(
        PhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var volunteer = await _dbContext.Volunteers
            .FirstOrDefaultAsync(v => v.PhoneNumber == phoneNumber, cancellationToken: cancellationToken);
        
        return volunteer ?? Result.Failure<Volunteer>("Phone number not found");
    }
}