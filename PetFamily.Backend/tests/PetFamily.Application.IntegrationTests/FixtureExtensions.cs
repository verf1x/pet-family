using AutoFixture;
using Bogus;
using PetFamily.Application.VolunteersManagement.UseCases.Create;
using PetFamily.Contracts.Dtos;
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
}