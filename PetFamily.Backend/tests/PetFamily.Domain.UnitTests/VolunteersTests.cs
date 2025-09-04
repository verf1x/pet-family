using CSharpFunctionalExtensions;
using FluentAssertions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Domain.UnitTests;

public class VolunteersTests
{
    [Fact]
    public void AddPet_First_Approach_ReturnsSuccessResult()
    {
        // Arrange
        Volunteer volunteer = GetUniqueVolunteer();
        Pet pet = GetUniquePet();

        // Act
        UnitResult<Error> result = volunteer.AddPet(pet);

        // Assert
        result.IsSuccess.Should().BeTrue();
        volunteer.Pets[^1].Id.Should().BeEquivalentTo(pet.Id);
        volunteer.Pets[^1].Position.Should().BeEquivalentTo(Position.First);
    }

    [Fact]
    public void AddPet_With_Other_Pets_ReturnsSuccessResult()
    {
        // Arrange
        const int petsCount = 5;
        Volunteer volunteer = GetUniqueVolunteer();
        IEnumerable<Pet> pets = Enumerable.Range(1, petsCount).Select(_ => GetUniquePet());

        foreach (Pet pet in pets)
        {
            volunteer.AddPet(pet);
        }

        Pet petToAdd = GetUniquePet();

        // Act
        UnitResult<Error> result = volunteer.AddPet(petToAdd);
        Result<Position, Error> serialNumber = Position.Create(petsCount + 1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        volunteer.Pets[^1].Id.Should().BeEquivalentTo(petToAdd.Id);
        volunteer.Pets[^1].Position.Should().BeEquivalentTo(serialNumber.Value);
    }

    [Fact]
    public void MovePet_Should_Not_Move_When_Pet_Already_At_New_Position()
    {
        // Arrange
        const int petsCount = 6;
        Volunteer volunteer = CreateVolunteerWithPets(petsCount);

        Position? secondPosition = Position.Create(2).Value;

        Pet firstPet = volunteer.Pets[0];
        Pet secondPet = volunteer.Pets[1];
        Pet thirdPet = volunteer.Pets[2];
        Pet fourthPet = volunteer.Pets[3];
        Pet fifthPet = volunteer.Pets[4];
        Pet sixthPet = volunteer.Pets[5];

        // Act
        UnitResult<Error> result = volunteer.MovePet(secondPet, secondPosition);

        // Assert
        result.IsSuccess.Should().BeTrue();

        firstPet.Position.Should().Be(Position.Create(1).Value);
        secondPet.Position.Should().Be(Position.Create(2).Value);
        thirdPet.Position.Should().Be(Position.Create(3).Value);
        fourthPet.Position.Should().Be(Position.Create(4).Value);
        fifthPet.Position.Should().Be(Position.Create(5).Value);
        sixthPet.Position.Should().Be(Position.Create(6).Value);
    }

    [Fact]
    public void MovePet_Should_Move_Other_Pets_Forward_When_New_Position_Is_Lower()
    {
        // Arrange
        const int petsCount = 5;
        Volunteer volunteer = CreateVolunteerWithPets(petsCount);

        Position? secondPosition = Position.Create(2).Value;

        Pet firstPet = volunteer.Pets[0];
        Pet secondPet = volunteer.Pets[1];
        Pet thirdPet = volunteer.Pets[2];
        Pet fourthPet = volunteer.Pets[3];
        Pet fifthPet = volunteer.Pets[4];

        // Act
        UnitResult<Error> result = volunteer.MovePet(fourthPet, secondPosition);

        // Assert
        result.IsSuccess.Should().BeTrue();

        firstPet.Position.Should().Be(Position.Create(1).Value);
        secondPet.Position.Should().Be(Position.Create(3).Value);
        thirdPet.Position.Should().Be(Position.Create(4).Value);
        fourthPet.Position.Should().Be(Position.Create(2).Value);
        fifthPet.Position.Should().Be(Position.Create(5).Value);
    }

    [Fact]
    public void MovePet_Should_Move_Other_Pets_Backward_When_New_Position_Is_Greater()
    {
        // Arrange
        const int petsCount = 5;
        Volunteer volunteer = CreateVolunteerWithPets(petsCount);

        Position? fourthPosition = Position.Create(4).Value;

        Pet firstPet = volunteer.Pets[0];
        Pet secondPet = volunteer.Pets[1];
        Pet thirdPet = volunteer.Pets[2];
        Pet fourthPet = volunteer.Pets[3];
        Pet fifthPet = volunteer.Pets[4];

        // Act
        UnitResult<Error> result = volunteer.MovePet(secondPet, fourthPosition);

        // Assert
        result.IsSuccess.Should().BeTrue();

        firstPet.Position.Should().Be(Position.Create(1).Value);
        secondPet.Position.Should().Be(Position.Create(4).Value);
        thirdPet.Position.Should().Be(Position.Create(2).Value);
        fourthPet.Position.Should().Be(Position.Create(3).Value);
        fifthPet.Position.Should().Be(Position.Create(5).Value);
    }

    [Fact]
    public void MovePet_Should_Move_Other_Pets_Forward_When_New_Position_Is_First()
    {
        // Arrange
        const int petsCount = 5;
        Volunteer volunteer = CreateVolunteerWithPets(petsCount);

        Position? firstPosition = Position.Create(1).Value;

        Pet firstPet = volunteer.Pets[0];
        Pet secondPet = volunteer.Pets[1];
        Pet thirdPet = volunteer.Pets[2];
        Pet fourthPet = volunteer.Pets[3];
        Pet fifthPet = volunteer.Pets[4];

        // Act
        UnitResult<Error> result = volunteer.MovePet(fifthPet, firstPosition);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Should().Be(Position.Create(2).Value);
        secondPet.Position.Should().Be(Position.Create(3).Value);
        thirdPet.Position.Should().Be(Position.Create(4).Value);
        fourthPet.Position.Should().Be(Position.Create(5).Value);
        fifthPet.Position.Should().Be(Position.Create(1).Value);
    }

    [Fact]
    public void MovePet_Should_Move_Other_Pets_Backward_When_New_Position_Is_Last()
    {
        // Arrange
        const int petsCount = 5;
        Volunteer volunteer = CreateVolunteerWithPets(petsCount);

        Position? fifthPosition = Position.Create(5).Value;

        Pet firstPet = volunteer.Pets[0];
        Pet secondPet = volunteer.Pets[1];
        Pet thirdPet = volunteer.Pets[2];
        Pet fourthPet = volunteer.Pets[3];
        Pet fifthPet = volunteer.Pets[4];

        // Act
        UnitResult<Error> result = volunteer.MovePet(firstPet, fifthPosition);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Should().Be(Position.Create(5).Value);
        secondPet.Position.Should().Be(Position.Create(1).Value);
        thirdPet.Position.Should().Be(Position.Create(2).Value);
        fourthPet.Position.Should().Be(Position.Create(3).Value);
        fifthPet.Position.Should().Be(Position.Create(4).Value);
    }

    private Volunteer CreateVolunteerWithPets(int petsCount)
    {
        Volunteer volunteer = GetUniqueVolunteer();
        IEnumerable<Pet> pets = Enumerable.Range(1, petsCount).Select(_ => GetUniquePet());

        foreach (Pet pet in pets)
        {
            volunteer.AddPet(pet);
        }

        return volunteer;
    }

    private Email GetRandomEmail()
    {
        string emailLine = $"{Guid.NewGuid().ToString("N")[..8]}@petfamily.com";

        return Email.Create(emailLine).Value;
    }

    private PhoneNumber GetRandomPhoneNumber()
    {
        string phoneNumber = $"+7{string.Concat(Enumerable.Range(0, 10)
            .Select(_ => Random.Shared.Next(0, 10)))}";

        return PhoneNumber.Create(phoneNumber).Value;
    }

    private Volunteer GetUniqueVolunteer()
    {
        FullName? fullName = FullName.Create("John", "Doe").Value;
        Email email = GetRandomEmail();
        Description? description = Description.Create("description").Value;
        Experience? experience = Experience.Create(5).Value;
        PhoneNumber phoneNumber = GetRandomPhoneNumber();
        List<SocialNetwork> socialNetworks = new(
            [
                SocialNetwork.Create("string", "https://exampleone.com/johndoe").Value,
                SocialNetwork.Create("string", "https://exampletwo.com/johndoe").Value
            ]
        );
        List<HelpRequisite> helpRequisites = new(
        [
            HelpRequisite.Create("string", "string").Value,
            HelpRequisite.Create("string", "string").Value
        ]);

        return new Volunteer(
            VolunteerId.CreateNew(),
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            socialNetworks,
            helpRequisites);
    }

    private Pet GetUniquePet()
    {
        Nickname? nickname = Nickname.Create("JohnDoe").Value;
        Description? description = Description.Create("string").Value;
        SpeciesBreed? speciesBreed = SpeciesBreed.Create(SpeciesId.CreateNew(), BreedId.CreateNew()).Value;
        Color? color = Color.Create("Red").Value;
        HealthInfo? healthInfo = HealthInfo.Create(
            "Healthy",
            true,
            false).Value;
        Address? address = Address.Create(
            [
                "123 Main St",
                "Apt 4B",
                "Springfield"
            ],
            "USA",
            "Illinois",
            "12345",
            "US"
        ).Value;
        Measurements? measurements = Measurements.Create(
            50,
            17).Value;
        PhoneNumber phoneNumber = GetRandomPhoneNumber();
        DateOnly dateOfBirth = new(2022, 1, 1);
        List<HelpRequisite> helpRequisites = new(
        [
            HelpRequisite.Create("string", "string").Value,
            HelpRequisite.Create("string", "string").Value
        ]);

        return new Pet(
            PetId.CreateNew(),
            nickname,
            description,
            speciesBreed,
            color,
            healthInfo,
            address,
            measurements,
            phoneNumber,
            dateOfBirth,
            HelpStatus.LookingForHome,
            helpRequisites);
    }
}