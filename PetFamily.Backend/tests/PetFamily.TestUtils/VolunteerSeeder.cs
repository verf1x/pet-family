using Bogus;
using PetFamily.Framework.EntityIds;
using PetFamily.Framework.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;
using Volunteers.Application.VolunteersManagement;
using Volunteers.Infrastructure.Postgres.DbContexts;

namespace PetFamily.TestUtils;

public static class VolunteerSeeder
{
    public static async Task<Volunteer> SeedVolunteerAsync(IVolunteersRepository repository, VolunteersWriteDbContext dbContext)
    {
        var faker = new Faker();

        var volunteerId = VolunteerId.CreateNew();
        var fullName = FullName.Create(
                faker.Name.FirstName(),
                faker.Name.LastName())
            .Value;
        var email = Email.Create(faker.Internet.Email()).Value;
        var description = Description.Create(faker.Lorem.Paragraph()).Value;
        var experience = Experience.Create(faker.Random.Int(0, 30)).Value;
        var phoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("############")).Value;
        var socialNetworks = new List<SocialNetwork>
        {
            SocialNetwork.Create(
                    faker.Internet.DomainName(),
                    faker.Internet.Url())
                .Value,
        };
        var helpRequisites = new List<HelpRequisite>
        {
            HelpRequisite.Create(
                    faker.Finance.AccountName(),
                    faker.Finance.CreditCardNumber())
                .Value,
        };

        var volunteer = new Volunteer(
            volunteerId,
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            socialNetworks,
            helpRequisites);

        await repository.AddAsync(volunteer);
        await dbContext.SaveChangesAsync();

        return volunteer;
    }

    public static async Task<Pet> SeedPetAsync(
        VolunteersWriteDbContext writeDbContext,
        Volunteer volunteer,
        SpeciesId speciesId,
        BreedId breedId)
    {
        var faker = new Faker();

        var petId = PetId.CreateNew();
        var nickname = Nickname.Create(faker.Name.FirstName()).Value;
        var description = Description.Create(faker.Lorem.Paragraph()).Value;
        var speciesBreed = SpeciesBreed.Create(speciesId, breedId).Value;
        var color = Color.Create(faker.Commerce.Color()).Value;
        var healthInfo = HealthInfo.Create(faker.Lorem.Sentence(), true, true).Value;
        var address = Address.Create(
                new List<string>(
                    [faker.Address.StreetAddress(), faker.Address.City()]),
                faker.Address.Country(),
                faker.Address.State(),
                faker.Address.ZipCode(),
                faker.Address.CountryCode())
            .Value;
        var measurements = Measurements.Create(
            faker.Random.Float(.05F, 70.0F),
            faker.Random.Float(.05F, 70.0F)).Value;
        var ownerPhoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("############")).Value;
        var dateOfBirth = faker.Date.PastDateOnly(30);
        var helpStatus = HelpStatus.NeedsHelp;
        var helpRequisites = new List<HelpRequisite>
        {
            HelpRequisite.Create(
                    faker.Finance.AccountName(),
                    faker.Finance.CreditCardNumber())
                .Value,
        };

        var pet = new Pet(
            petId,
            nickname,
            description,
            speciesBreed,
            color,
            healthInfo,
            address,
            measurements,
            ownerPhoneNumber,
            dateOfBirth,
            helpStatus,
            helpRequisites);

        volunteer.AddPet(pet);

        await writeDbContext.SaveChangesAsync();

        return pet;
    }
}