using CSharpFunctionalExtensions;
using PetFamily.Domain.ValueObjects;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Domain.Entities;

public class Volunteer : Shared.Entity<VolunteerId>
{
    private readonly List<SocialNetwork> _socialNetworks = [];
    private readonly List<Pet> _allPets = [];
    
    public FullName FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int ExperienceYears { get; private set; }
    public IReadOnlyList<Pet> PetsNeedsHelp => GetPetsNeedsHelp();
    public IReadOnlyList<Pet> PetsLookingForHome => GetPetsLookingForHome();
    public IReadOnlyList<Pet> PetsFoundHome => GetPetsFoundHome();
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks;
    public HelpDetails HelpDetails { get; private set; } = null!;
    public IReadOnlyList<Pet> AllPets => _allPets;
    
    // ef core ctor
    private Volunteer(VolunteerId id) : base(id) { }
    
    private Volunteer(
        VolunteerId id,
        FullName fullName,
        string email,
        string description,
        int experienceYears,
        PhoneNumber phoneNumber,
        HelpDetails helpDetails) : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        ExperienceYears = experienceYears;
        PhoneNumber = phoneNumber;
        HelpDetails = helpDetails;
    }
    
    public static Result<Volunteer> Create(
        VolunteerId id,
        FullName fullName,
        string email,
        string description,
        int experienceYears,
        PhoneNumber phoneNumber,
        HelpDetails helpDetails)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Volunteer>("Email cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Volunteer>("Description cannot be empty.");
        
        if (experienceYears < 0)
            return Result.Failure<Volunteer>("Experience years cannot be negative.");
        
        Volunteer volunteer = new(id, fullName, email, description, experienceYears, phoneNumber, helpDetails);
        
        return Result.Success(volunteer);
    }
    
    public Result AddSocialNetwork(SocialNetwork socialNetwork)
    {
        if (_socialNetworks.Contains(socialNetwork))
            return Result.Failure("Social network already exists in the volunteer's list.");
        
        _socialNetworks.Add(socialNetwork);
        
        return Result.Success();
    }
    
    public Result AddPet(Pet pet)
    {
        if (_allPets.Contains(pet))
            return Result.Failure("Pet already exists in the volunteer's list.");
        
        _allPets.Add(pet);
        
        return Result.Success();
    }
    
    private IReadOnlyList<Pet> GetPetsNeedsHelp()
    {
        return _allPets
            .Where(p => p.HelpStatus is HelpStatus.NeedsHelp)
            .ToList();
    }
    
    private IReadOnlyList<Pet> GetPetsLookingForHome()
    {
        return _allPets
            .Where(p => p.HelpStatus is HelpStatus.LookingForHome)
            .ToList();
    }
    
    private IReadOnlyList<Pet> GetPetsFoundHome()
    {
        return _allPets
            .Where(p => p.HelpStatus is HelpStatus.FoundHome)
            .ToList();
    }
}