using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteersManagement.Enums;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Domain.VolunteersManagement.Entities;

public class Pet : SoftDeletableEntity<PetId>
{
    private readonly List<HelpRequisite> _helpRequisites;
    private readonly List<Photo> _photos;
    
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

    public IReadOnlyList<HelpRequisite> HelpRequisites => _helpRequisites;

    public IReadOnlyList<Photo> Photos => _photos;
    
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
        List<HelpRequisite> helpRequisites,
        List<Photo> photos) : base(id)
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
        _helpRequisites = helpRequisites;
        _photos = photos;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void AddPhotos(List<Photo> photos)
    {
        foreach (var photo in photos)
        {
            if(!_photos.Contains(photo))
                _photos.Add(photo);
        }
    }
    
    public void RemovePhotos(List<Photo> photos)
    {
        foreach (var photo in photos)
            _photos.Remove(photo);
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