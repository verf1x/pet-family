using AutoFixture;
using Bogus;
using PetFamily.Application.VolunteersManagement.UseCases.AddPet;
using PetFamily.Application.VolunteersManagement.UseCases.Create;
using PetFamily.Application.VolunteersManagement.UseCases.UpdateMainInfo;
using PetFamily.Application.VolunteersManagement.UseCases.UploadPetPhotos;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Contracts.Dtos.Volunteer;

namespace PetFamily.Application.IntegrationTests;

public static class FixtureExtensions
{
    public static CreateVolunteerCommand BuildCreateVolunteerCommand(
        this IFixture fixture,
        string? firstName = null,
        string? lastName = null,
        string? middleName = null,
        string? email = null,
        string? description = null,
        int? experienceYears = null,
        string? phoneNumber = null,
        IEnumerable<SocialNetworkDto>? socialNetworks = null,
        IEnumerable<HelpRequisiteDto>? helpRequisites = null)
    {
        var faker = new Faker();

        firstName ??= faker.Name.FirstName();
        lastName ??= faker.Name.LastName();
        email ??= faker.Internet.Email();
        description ??= faker.Lorem.Paragraph();
        experienceYears ??= faker.Random.Int(0, 30);
        phoneNumber ??= faker.Phone.PhoneNumber("############");
        socialNetworks ??= new List<SocialNetworkDto>
        {
            new SocialNetworkDto(faker.Internet.DomainName(), faker.Internet.Url()),
        };
        helpRequisites ??= new List<HelpRequisiteDto>
        {
            new HelpRequisiteDto(faker.Lorem.Word(), faker.Finance.CreditCardNumber()),
        };

        return fixture.Build<CreateVolunteerCommand>()
            .With(cv => cv.FullName, new FullNameDto(firstName, lastName, middleName))
            .With(cv => cv.Email, email)
            .With(cv => cv.Description, description)
            .With(cv => cv.ExperienceYears, experienceYears)
            .With(cv => cv.PhoneNumber, phoneNumber)
            .With(cv => cv.SocialNetworks, socialNetworks)
            .With(cv => cv.HelpRequisites, helpRequisites)
            .Create();
    }

    public static UpdateMainInfoCommand BuildUpdateVolunteerMainInfoCommand(
        this IFixture fixture,
        Guid volunteerId,
        string? firstName = null,
        string? lastName = null,
        string? middleName = null,
        string? email = null,
        string? description = null,
        int? experienceYears = null,
        string? phoneNumber = null)
    {
        var faker = new Faker();

        firstName ??= faker.Name.FirstName();
        lastName ??= faker.Name.LastName();
        email ??= faker.Internet.Email();
        description ??= faker.Lorem.Paragraph();
        experienceYears ??= faker.Random.Int(0, 30);
        phoneNumber ??= faker.Phone.PhoneNumber("############");

        return fixture.Build<UpdateMainInfoCommand>()
            .With(u => u.VolunteerId, volunteerId)
            .With(u => u.FullName, new FullNameDto(firstName, lastName, middleName))
            .With(u => u.Email, email)
            .With(u => u.Description, description)
            .With(u => u.ExperienceYears, experienceYears)
            .With(u => u.PhoneNumber, phoneNumber)
            .Create();
    }

    public static AddPetCommand BuildAddPetCommand(
        this IFixture fixture,
        Guid volunteerId,
        SpeciesBreedDto speciesBreed,
        string? nickname = null,
        string? description = null,
        string? color = null,
        string? healthStatus = null,
        IEnumerable<string>? addressLines = null,
        string? locality = null,
        string? countryCode = null,
        float? height = null,
        float? weight = null,
        string? ownerPhoneNumber = null,
        DateOnly? dateOfBirth = null,
        int? helpStatus = null,
        IEnumerable<HelpRequisiteDto>? helpRequisites = null)
    {
        var faker = new Faker();

        nickname ??= faker.Name.FirstName();
        description ??= faker.Lorem.Paragraph();
        color ??= faker.Commerce.Color();
        healthStatus ??= faker.Lorem.Sentence();
        addressLines ??= new List<string> { faker.Address.StreetAddress(), faker.Address.SecondaryAddress() };
        locality ??= faker.Address.City();
        countryCode ??= faker.Address.CountryCode();
        height ??= faker.Random.Float(.05F, 80.0F);
        weight ??= faker.Random.Float(.1F, 70.0F);
        ownerPhoneNumber ??= faker.Phone.PhoneNumber("############");
        dateOfBirth ??= DateOnly.FromDateTime(faker.Date.Past(25));
        helpStatus ??= faker.Random.Int(0, 2);
        helpRequisites ??= new List<HelpRequisiteDto>
        {
            new HelpRequisiteDto(faker.Lorem.Word(), faker.Finance.CreditCardNumber()),
        };

        return fixture.Build<AddPetCommand>()
            .With(ap => ap.VolunteerId, volunteerId)
            .With(ap => ap.Nickname, nickname)
            .With(ap => ap.Description, description)
            .With(ap => ap.SpeciesBreedDto, speciesBreed)
            .With(ap => ap.Color, color)
            .With(ap => ap.HealthInfoDto, new HealthInfoDto(healthStatus, true, true))
            .With(ap => ap.AddressDto, new AddressDto(addressLines, locality, null, null, countryCode))
            .With(ap => ap.MeasurementsDto, new MeasurementsDto(height.Value, weight.Value))
            .With(ap => ap.OwnerPhoneNumber, ownerPhoneNumber)
            .With(ap => ap.DateOfBirth, dateOfBirth.Value)
            .With(ap => ap.HelpStatus, helpStatus.Value)
            .With(ap => ap.HelpRequisites, helpRequisites)
            .Create();
    }

    public static UploadPetPhotosCommand BuildUploadPetPhotosCommand(
        this IFixture fixture,
        Guid volunteerId,
        Guid petId,
        IEnumerable<UploadFileDto>? photos = null)
    {
        var faker = new Faker();

        photos ??= new List<UploadFileDto>
        {
            new(GetFakeImageStream(), faker.Image.DataUri(8, 8)),
            new(GetFakeImageStream(), faker.Image.DataUri(8, 8)),
        };

        fixture.Register<Stream>(() =>
        {
            var random = new Random();
            byte[] fakeImageContent = new byte[64];
            random.NextBytes(fakeImageContent);
            return new MemoryStream(fakeImageContent);
        });

        return fixture.Build<UploadPetPhotosCommand>()
            .With(up => up.VolunteerId, volunteerId)
            .With(up => up.PetId, petId)
            .With(up => up.Photos, photos)
            .Create();
    }

    private static Stream GetFakeImageStream()
    {
        var random = new Random();

        byte[] fakeImageContent = new byte[256];
        random.NextBytes(fakeImageContent);

        return new MemoryStream(fakeImageContent);
    }
}