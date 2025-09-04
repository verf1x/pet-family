using System.Data;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Core.Database;
using PetFamily.Core.Files;
using PetFamily.Core.Messaging;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;
using Volunteers.Application.VolunteersManagement;
using Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;
using Volunteers.Contracts.Dtos;

namespace PetFamily.Application.UnitTests;

public class UploadPetPhotosTests
{
    private readonly Mock<IFileProvider> _fileProviderMock = new();
    private readonly Mock<ILogger<UploadPetPhotosHandler>> _loggerMock = new();
    private readonly Mock<IMessageQueue<IEnumerable<string>>> _messageQueueMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IValidator<UploadPetPhotosCommand>> _validatorMock = new();
    private readonly Mock<IVolunteersRepository> _volunteersRepositoryMock = new();

    [Fact]
    public async Task HandlerShould_UploadPhotos_ToPet()
    {
        // Arrange
        Volunteer volunteer = GetUniqueVolunteer();
        Pet pet = GetUniquePet();
        volunteer.AddPet(pet);

        MemoryStream stream = new();
        const string fileName = "test.jpg";

        UploadFileDto uploadFileDto = new(stream, fileName);
        List<UploadFileDto> files = [uploadFileDto, uploadFileDto];

        List<string> photoPaths =
        [
            "test.jpg",
            "test.jpg"
        ];

        UploadPetPhotosCommand command = new(volunteer.Id, pet.Id, files);
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        _fileProviderMock
            .Setup(f => f.UploadPhotosAsync(It.IsAny<List<PhotoData>>(), cancellationToken))
            .ReturnsAsync(Result.Success<List<string>, Error>(photoPaths));

        _volunteersRepositoryMock.Setup(v => v.GetByIdAsync(volunteer.Id, cancellationToken))
            .ReturnsAsync(volunteer);

        _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(Mock.Of<IDbTransaction>());

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, cancellationToken))
            .ReturnsAsync(new ValidationResult());

        _messageQueueMock
            .Setup(m => m.WriteAsync(
                It.IsAny<IEnumerable<string>>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _messageQueueMock
            .Setup(m => m.ReadAsync(cancellationToken))
            .ReturnsAsync(It.IsAny<IEnumerable<string>>());

        UploadPetPhotosHandler handler = new UploadPetPhotosHandler(
            _fileProviderMock.Object,
            _unitOfWorkMock.Object,
            _volunteersRepositoryMock.Object,
            _validatorMock.Object,
            _messageQueueMock.Object,
            _loggerMock.Object);

        // Act
        Result<List<string>, ErrorList> result = await handler.HandleAsync(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(photoPaths);
        volunteer.Pets.First(i => i.Id == pet.Id).Photos.Should().HaveCount(2);
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
        List<Photo> photos = new();

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
}