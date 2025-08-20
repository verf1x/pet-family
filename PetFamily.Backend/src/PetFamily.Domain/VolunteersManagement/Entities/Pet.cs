using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteersManagement.Enums;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Domain.VolunteersManagement.Entities;

public class Pet : SoftDeletableEntity<PetId>
{
    private readonly List<HelpRequisite> _helpRequisites = [];
    private readonly List<Photo> _photos = [];

    public Nickname Nickname { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public Position Position { get; private set; } = null!;

    public SpeciesBreed SpeciesBreed { get; private set; } = null!;

    public Color Color { get; private set; } = null!;

    public HealthInfo HealthInfo { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public Measurements Measurements { get; private set; } = null!;

    public PhoneNumber OwnerPhoneNumber { get; private set; } = null!;

    public DateOnly DateOfBirth { get; private set; }

    public HelpStatus HelpStatus { get; private set; }

    public IReadOnlyList<HelpRequisite> HelpRequisites => _helpRequisites;

    public IReadOnlyList<Photo> Photos => _photos;

    public Photo? MainPhoto { get; private set; }

    public DateTime CreatedAt { get; private set; }

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
        List<HelpRequisite> helpRequisites)
        : base(id)
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
        CreatedAt = DateTime.UtcNow;
    }

    // ef core
    private Pet(PetId id)
        : base(id)
    {
    }

    public void AddPhotos(List<Photo> photos)
    {
        foreach (var photo in photos)
        {
            if (!_photos.Contains(photo))
                _photos.Add(photo);
        }
    }

    public void RemovePhotos(List<Photo> photos)
    {
        foreach (var photo in photos)
            _photos.Remove(photo);
    }

    public UnitResult<Error> SetMainPhoto(Photo photo)
    {
        var candidatePhoto = _photos.FirstOrDefault(p => p == photo);

        if (candidatePhoto is null)
            return Errors.General.NotFound();

        MainPhoto = candidatePhoto;

        return Result.Success<Error>();
    }

    public void SetPosition(Position position)
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

    public void UpdateMainInfo(
        Nickname nickname,
        Description description,
        SpeciesBreed speciesBreed,
        Color color,
        HealthInfo healthInfo,
        Address address,
        Measurements measurements,
        PhoneNumber ownerPhoneNumber,
        DateOnly dateOfBirth,
        List<HelpRequisite> helpRequisites)
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

        _helpRequisites.Clear();
        _helpRequisites.AddRange(helpRequisites);
    }

    public void UpdateHelpStatus(HelpStatus helpStatus)
    {
        HelpStatus = helpStatus;
    }
}