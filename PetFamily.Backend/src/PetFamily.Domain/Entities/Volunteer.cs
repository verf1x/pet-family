using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Pet;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Domain.Entities;

public class Volunteer : SoftDeletableEntity<VolunteerId>
{
    private readonly List<Pet> _pets = [];
    
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
    public IReadOnlyList<Pet> Pets => _pets;
    
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
    
    public UnitResult<Error> AddPet(Pet pet)
    {
        if (_pets.Contains(pet))
            return Errors.General.Conflict(pet.Id.Value);
        
        _pets.Add(pet);

        return UnitResult.Success<Error>();
    }

    public void UpdateMainInfo(
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        PhoneNumber phoneNumber)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
    }
    
    private IReadOnlyList<Pet> GetPetsNeedsHelp()
    {
        return _pets
            .Where(p => p.HelpStatus is HelpStatus.NeedsHelp)
            .ToList();
    }
    
    private IReadOnlyList<Pet> GetPetsLookingForHome()
    {
        return _pets
            .Where(p => p.HelpStatus is HelpStatus.LookingForHome)
            .ToList();
    }
    
    private IReadOnlyList<Pet> GetPetsFoundHome()
    {
        return _pets
            .Where(p => p.HelpStatus is HelpStatus.FoundHome)
            .ToList();
    }
}