using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Species;
using PetFamily.Domain.ValueObjects.Pet;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Domain.Entities;

public class Pet : Shared.Entity<PetId>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public SpeciesBreed SpeciesBreed { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public string HealthStatus { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public int Weight { get; private set; }
    public int Height { get; private set; }
    public PhoneNumber OwnerPhoneNumber { get; private set; } = null!;
    public bool IsNeutered { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public bool IsVaccinated { get; private set; }
    public HelpStatus HelpStatus { get; private set; }
    public HelpRequisites HelpRequisites { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    
    // ef core ctor
    private Pet(PetId id) : base(id) { }

    private Pet(
        PetId id,
        string name,
        string description,
        SpeciesBreed speciesBreed,
        string color,
        string healthStatus,
        Address address,
        int weight,
        int height,
        PhoneNumber ownerPhoneNumber,
        bool isNeutered,
        DateOnly dateOfBirth,
        bool isVaccinated,
        HelpStatus helpStatus) : base(id)
    {
        Name = name;
        Description = description;
        SpeciesBreed = speciesBreed;
        Color = color;
        HealthStatus = healthStatus;
        Address = address;
        Weight = weight;
        Height = height;
        OwnerPhoneNumber = ownerPhoneNumber;
        IsNeutered = isNeutered;
        DateOfBirth = dateOfBirth;
        IsVaccinated = isVaccinated;
        HelpStatus = helpStatus;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Pet, Error> Create(
        PetId id,
        string name,
        string description,
        SpeciesBreed speciesBreed,
        string color,
        string healthStatus,
        Address address,
        int weight,
        int height,
        PhoneNumber ownerPhoneNumber,
        bool isNeutered,
        DateOnly dateOfBirth,
        bool isVaccinated,
        HelpStatus helpStatus)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Errors.General.ValueIsRequired(nameof(name));
        
        if(string.IsNullOrWhiteSpace(description))
            return Errors.General.ValueIsRequired(nameof(description));
        
        if(string.IsNullOrWhiteSpace(color))
            return Errors.General.ValueIsRequired(nameof(color));
        
        if(string.IsNullOrWhiteSpace(healthStatus))
            return Errors.General.ValueIsRequired(nameof(healthStatus));
        
        if(weight <= 0)
            return Errors.General.ValueIsInvalid(nameof(weight));
        
        if(height <= 0)
            return Errors.General.ValueIsInvalid(nameof(height));
        
        return new Pet(id, name, description, speciesBreed, color, healthStatus, address, weight,
            height, ownerPhoneNumber, isNeutered, dateOfBirth, isVaccinated, helpStatus);
    }
}

public enum HelpStatus
{
    NeedsHelp,
    LookingForHome,
    FoundHome
}