using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Species.ValueObjects;
using PetFamily.Domain.Volunteers.Enums;
using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Domain.Volunteers.Entities;

public class Pet : SoftDeletableEntity<PetId>
{
    public Nickname Nickname { get; private set; }
    
    public Description Description { get; private set; }
    
    public Position Position { get; private set; }
    
    public SpeciesBreed SpeciesBreed { get; private set; }
    
    public Color Color { get; private set; }
    
    public HealthInfo HealthInfo { get; private set; }
    
    public Address Address { get; private set; }
    
    public Measurements Measurements { get; private set; }
    
    public PhoneNumber OwnerPhoneNumber { get; private set; }
    
    public DateOnly DateOfBirth { get; private set; }
    
    public HelpStatus HelpStatus { get; private set; }
    
    public ValueObjectList<HelpRequisite> HelpRequisites { get; private set; }
    
    public ValueObjectList<Photo> Photos { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    // ef core ctor
    private Pet(PetId id) : base(id) { }

    public Pet(
        PetId id,
        Nickname nickname,
        Description description,
        SpeciesBreed speciesBreed, 
        Color color,
        HealthInfo healthInfo,
        Address address,
        Measurements measurements,
        PhoneNumber ownerPhoneNumber,
        DateOnly dateOfBirth,
        HelpStatus helpStatus,
        ValueObjectList<HelpRequisite> helpRequisites,
        ValueObjectList<Photo> photos) : base(id)
    {
        Nickname = nickname;
        Description = description;
        SpeciesBreed = speciesBreed;
        Color = color;
        HealthInfo = healthInfo;
        Address = address;
        Measurements = measurements;
        OwnerPhoneNumber = ownerPhoneNumber;
        DateOfBirth = dateOfBirth;
        HelpStatus = helpStatus;
        HelpRequisites = helpRequisites;
        Photos = photos;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void SetSerialNumber(Position position)
    {
        Position = position;
    }

    public UnitResult<Error> MoveForward()
    {
        var newPosition = Position.Forward();
        if (newPosition.IsFailure)
            return newPosition.Error;
        
        Position = newPosition.Value;
        
        return Result.Success<Error>();
    }
    
    public UnitResult<Error> MoveBackward()
    {
        var newPosition = Position.Backward();
        if (newPosition.IsFailure)
            return newPosition.Error;
        
        Position = newPosition.Value;
        
        return Result.Success<Error>();
    }
    
    public void Move(Position newPosition)
    {
        Position = newPosition;
    }
}