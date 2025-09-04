using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.Entities;

public class Volunteer : SoftDeletableEntity<VolunteerId>
{
    private readonly List<Pet> _pets = [];

    public Volunteer(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        PhoneNumber phoneNumber,
        List<SocialNetwork> socialNetworks,
        List<HelpRequisite> helpRequisites)
        : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        SocialNetworks = socialNetworks;
        HelpRequisites = helpRequisites;
    }

    // ef core
    private Volunteer(VolunteerId id)
        : base(id)
    {
    }

    public FullName FullName { get; private set; } = null!;

    public Email Email { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public Experience Experience { get; private set; } = null!;

    public PhoneNumber PhoneNumber { get; private set; } = null!;

    public IReadOnlyList<SocialNetwork> SocialNetworks { get; private set; } = null!;

    public IReadOnlyList<HelpRequisite> HelpRequisites { get; private set; } = null!;

    public IReadOnlyList<Pet> Pets => _pets;

    public Result<Pet, Error> GetPetById(PetId petId)
    {
        Pet? pet = _pets.FirstOrDefault(p => p.Id == petId);
        if (pet is null)
        {
            return Errors.General.NotFound(petId.Value);
        }

        return pet;
    }

    public Result<Guid, Error> RemovePet(Pet pet)
    {
        if (!_pets.Contains(pet))
        {
            return Errors.General.NotFound(pet.Id.Value);
        }

        _pets.Remove(pet);

        AdjustPetsPositions();

        return pet.Id.Value;
    }

    public UnitResult<Error> AddPet(Pet pet)
    {
        if (_pets.Contains(pet))
        {
            return Errors.General.Conflict(pet.Id.Value);
        }

        Result<Position, Error> serialNumberResult = Position.Create(_pets.Count + 1);
        if (serialNumberResult.IsFailure)
        {
            return serialNumberResult.Error;
        }

        pet.SetPosition(serialNumberResult.Value);

        _pets.Add(pet);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        Position currentPosition = pet.Position;

        if (currentPosition == newPosition || _pets.Count == 1)
        {
            return Result.Success<Error>();
        }

        Result<Position, Error> adjustedPositionResult = AdjustNewPositionIfOutOfRange(newPosition);
        if (adjustedPositionResult.IsFailure)
        {
            return adjustedPositionResult.Error;
        }

        newPosition = adjustedPositionResult.Value;

        UnitResult<Error> moveResult = MovePetsBetweenPositions(newPosition, currentPosition);
        if (moveResult.IsFailure)
        {
            return moveResult.Error;
        }

        pet.Move(newPosition);

        return Result.Success<Error>();
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

    public IReadOnlyList<Pet> GetPetsNeedsHelp() =>
        _pets
            .Where(p => p.HelpStatus is HelpStatus.NeedsHelp)
            .ToList();

    public IReadOnlyList<Pet> GetPetsLookingForHome() =>
        _pets
            .Where(p => p.HelpStatus is HelpStatus.LookingForHome)
            .ToList();

    public IReadOnlyList<Pet> GetPetsFoundHome() =>
        _pets
            .Where(p => p.HelpStatus is HelpStatus.FoundHome)
            .ToList();

    private void AdjustPetsPositions()
    {
        for (int i = 0; i < _pets.Count; i++)
        {
            _pets[i].SetPosition(Position.Create(i + 1).Value);
        }
    }

    private UnitResult<Error> MovePetsBetweenPositions(Position newPosition, Position currentPosition)
    {
        if (newPosition < currentPosition)
        {
            IEnumerable<Pet> petsToMove = _pets.Where(i => i.Position >= newPosition
                                                           && i.Position <= currentPosition);

            foreach (Pet petToMove in petsToMove)
            {
                UnitResult<Error> result = petToMove.MoveForward();
                if (result.IsFailure)
                {
                    return result.Error;
                }
            }
        }
        else if (newPosition > currentPosition)
        {
            IEnumerable<Pet> petsToMove = _pets.Where(i => i.Position > currentPosition
                                                           && i.Position <= newPosition);

            foreach (Pet petToMove in petsToMove)
            {
                UnitResult<Error> result = petToMove.MoveBackward();
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
        {
            return newPosition;
        }

        Result<Position, Error> lastPositionResult = Position.Create(_pets.Count - 1);
        if (lastPositionResult.IsFailure)
        {
            return lastPositionResult.Error;
        }

        return lastPositionResult.Value;
    }
}