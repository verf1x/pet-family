using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Pet;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Domain.Entities;

public class Volunteer : Shared.Entity<VolunteerId>
{
    private readonly List<Pet> _allPets = [];
    
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public Description Description { get; private set; }
    public Experience Experience { get; private set; }
    public IReadOnlyList<Pet> PetsNeedsHelp => GetPetsNeedsHelp();
    public IReadOnlyList<Pet> PetsLookingForHome => GetPetsLookingForHome();
    public IReadOnlyList<Pet> PetsFoundHome => GetPetsFoundHome();
    public PhoneNumber PhoneNumber { get; private set; }
    public SocialNetworks SocialNetworks { get; private set; }
    public HelpRequisites HelpRequisites { get; private set; }
    public IReadOnlyList<Pet> AllPets => _allPets;
    
    // ef core ctor
    private Volunteer(VolunteerId id) : base(id) { }
    
    public Volunteer(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        PhoneNumber phoneNumber,
        SocialNetworks socialNetworks,
        HelpRequisites helpRequisites) : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        SocialNetworks = socialNetworks;
        HelpRequisites = helpRequisites;
    }
    
    public Result<Error> AddPet(Pet pet)
    {
        if (_allPets.Contains(pet))
            return Errors.General.Conflict(pet.Id.Value);
        
        _allPets.Add(pet);
        
        return Result.Success<Error>(null!);
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