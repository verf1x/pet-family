using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Volunteers.Enums;
using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Domain.Volunteers.Entities;

public class Volunteer : SoftDeletableEntity<VolunteerId>
{
    private readonly List<Pet> _pets = [];

    public FullName FullName { get; private set; }

    public Email Email { get; private set; }

    public Description Description { get; private set; }

    public Experience Experience { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public SocialNetworks SocialNetworks { get; private set; }

    public HelpRequisites HelpRequisites { get; private set; }

    public IReadOnlyList<Pet> Pets => _pets;

    public IReadOnlyList<Pet> PetsNeedsHelp => GetPetsNeedsHelp();

    public IReadOnlyList<Pet> PetsLookingForHome => GetPetsLookingForHome();

    public IReadOnlyList<Pet> PetsFoundHome => GetPetsFoundHome();

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

        var serialNumberResult = Position.Create(_pets.Count + 1);
        if (serialNumberResult.IsFailure)
            return serialNumberResult.Error;

        pet.SetSerialNumber(serialNumberResult.Value);

        _pets.Add(pet);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        var currentPosition = pet.Position;

        if (currentPosition == newPosition || _pets.Count == 1)
            return Result.Success<Error>();

        var adjustedPositionResult = AdjustNewPositionIfOutOfRange(newPosition);
        if (adjustedPositionResult.IsFailure)
            return adjustedPositionResult.Error;

        newPosition = adjustedPositionResult.Value;

        var moveResult = MovePetsBetweenPositions(newPosition, currentPosition);
        if(moveResult.IsFailure)
            return moveResult.Error;

        pet.Move(newPosition);

        return Result.Success<Error>();
    }

    private UnitResult<Error> MovePetsBetweenPositions(Position newPosition, Position currentPosition)
    {
        if (newPosition < currentPosition)
        {
            var petsToMove = _pets.Where(i => i.Position >= newPosition
                                              && i.Position <= currentPosition);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveForward();
                if (result.IsFailure)
                {
                    return result.Error;
                }
            }
        }
        else if (newPosition > currentPosition)
        {
            var petsToMove = _pets.Where(i => i.Position > currentPosition
                                              && i.Position <= newPosition);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveBackward();
                if (result.IsFailure)
                {
                    return result.Error; 
                }
            }
        }

        return Result.Success<Error>();
    }

    private Result<Position, Error> AdjustNewPositionIfOutOfRange(Position newPosition)
    {
        if (newPosition <= _pets.Count)
            return newPosition;

        var lastPositionResult = Position.Create(_pets.Count - 1);
        if (lastPositionResult.IsFailure)
            return lastPositionResult.Error;

        return lastPositionResult.Value;
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

    public IReadOnlyList<Pet> GetPetsNeedsHelp()
    {
        return _pets
            .Where(p => p.HelpStatus is HelpStatus.NeedsHelp)
            .ToList();
    }

    public IReadOnlyList<Pet> GetPetsLookingForHome()
    {
        return _pets
            .Where(p => p.HelpStatus is HelpStatus.LookingForHome)
            .ToList();
    }

    public IReadOnlyList<Pet> GetPetsFoundHome()
    {
        return _pets
            .Where(p => p.HelpStatus is HelpStatus.FoundHome)
            .ToList();
    }
}