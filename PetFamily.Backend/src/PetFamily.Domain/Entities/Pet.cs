using CSharpFunctionalExtensions;
using PetFamily.Domain.ValueObjects;
using PetFamily.Domain.ValueObjects.Pet;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Domain.Entities;

public class Pet : Shared.Entity<PetId>
{
    private readonly List<HelpDetails> _helpDetails = []; 
    
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public BreedSpecies BreedSpecies { get; private set; } = null!;
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
    public IReadOnlyList<HelpDetails> HelpDetails => _helpDetails;
    public DateTime CreatedAt { get; private set; }
    
    // ef core ctor
    private Pet(PetId id) : base(id) { }

    private Pet(
        PetId id,
        string name,
        string description,
        BreedSpecies breedSpecies,
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
        BreedSpecies = breedSpecies;
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
        BreedSpecies breedSpecies,
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
            return Result.Failure<Pet>("Name cannot be empty");
        
        if(string.IsNullOrWhiteSpace(description))
            return Result.Failure<Pet>("Description cannot be empty");
        
        if(string.IsNullOrWhiteSpace(color))
            return Result.Failure<Pet>("Color cannot be empty");
        
        if(string.IsNullOrWhiteSpace(healthStatus))
            return Result.Failure<Pet>("Health status cannot be empty");
        
        if(weight <= 0)
            return Result.Failure<Pet>("Weight must be greater than 0");
        
        if(height <= 0)
            return Result.Failure<Pet>("Height must be greater than 0");
        
        Pet pet = new(id, name, description, breedSpecies, color, healthStatus, address, weight, height, ownerPhoneNumber,
            isNeutered, dateOfBirth, isVaccinated, helpStatus);
        
        return Result.Success(pet);
    }
    
    public Result AddHelpDetails(HelpDetails details)
    {
        if(_helpDetails.Contains(details))
            return Result.Failure("Details already exists in the list.");
        
        _helpDetails.Add(details);
        
        return Result.Success();
    }
}

public enum HelpStatus
{
    NeedsHelp,
    LookingForHome,
    FoundHome
}