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

    public async Task<Result<Volunteer, Error>> GetByIdAsync(VolunteerId volunteerId)
    {
        var volunteer = await _dbContext.Volunteers
            .Include(v => v.AllPets)
            .FirstOrDefaultAsync(v => v.Id == volunteerId);

        if (volunteer is null)
            return Errors.General.NotFound(volunteerId.Value);

        return volunteer;
    }

    public async Task<Result<Volunteer>> GetByEmailAsync(Email email)
    {
        var volunteer = await _dbContext.Volunteers
            .Include(v => v.AllPets)
            .FirstOrDefaultAsync(v => v.Email == email);
        
        return volunteer ?? Result.Failure<Volunteer>("Email number not found");
    }
}