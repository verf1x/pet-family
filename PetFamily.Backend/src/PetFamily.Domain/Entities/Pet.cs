using PetFamily.Domain.Shared;
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
    public HelpDetails HelpDetails { get; private set; } = null!;
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

    public static Result<Pet> Create(
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
            return "Name cannot be empty";
        
        if(string.IsNullOrWhiteSpace(description))
            return "Description cannot be empty";
        
        if(string.IsNullOrWhiteSpace(color))
            return "Color cannot be empty";
        
        if(string.IsNullOrWhiteSpace(healthStatus))
            return "Health status cannot be empty";
        
        if(weight <= 0)
            return "Weight must be greater than 0";
        
        if(height <= 0)
            return "Height must be greater than 0";
        
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