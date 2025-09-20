﻿using Bogus;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;
using Volunteers.Application.VolunteersManagement;
using Volunteers.Infrastructure.Postgres.DbContexts;

namespace PetFamily.TestUtils;

public static class VolunteerSeeder
{
    public static async Task<Volunteer> SeedVolunteerAsync(
        IVolunteersRepository repository,
        VolunteersWriteDbContext dbContext)
    {
        Faker faker = new();

        VolunteerId volunteerId = VolunteerId.CreateNew();
        FullName? fullName = FullName.Create(
                faker.Name.FirstName(),
                faker.Name.LastName())
            .Value;
        Email? email = Email.Create(faker.Internet.Email()).Value;
        Description? description = Description.Create(faker.Lorem.Paragraph()).Value;
        Experience? experience = Experience.Create(faker.Random.Int(0, 30)).Value;
        PhoneNumber? phoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("############")).Value;
        List<SocialNetwork> socialNetworks = new()
        {
            SocialNetwork.Create(
                    faker.Internet.DomainName(),
                    faker.Internet.Url())
                .Value,
        };
        List<HelpRequisite> helpRequisites = new()
        {
            HelpRequisite.Create(
                    faker.Finance.AccountName(),
                    faker.Finance.CreditCardNumber())
                .Value,
        };

        Volunteer volunteer = new(
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
        Faker faker = new();

        PetId petId = PetId.CreateNew();
        Nickname? nickname = Nickname.Create(faker.Name.FirstName()).Value;
        Description? description = Description.Create(faker.Lorem.Paragraph()).Value;
        SpeciesBreed? speciesBreed = SpeciesBreed.Create(speciesId, breedId).Value;
        Color? color = Color.Create(faker.Commerce.Color()).Value;
        HealthInfo? healthInfo = HealthInfo.Create(faker.Lorem.Sentence(), true, true).Value;
        Address? address = Address.Create(
                new List<string>(
                    [faker.Address.StreetAddress(), faker.Address.City()]),
                faker.Address.Country(),
                faker.Address.State(),
                faker.Address.ZipCode(),
                faker.Address.CountryCode())
            .Value;
        Measurements? measurements = Measurements.Create(
            faker.Random.Float(.05F, 70.0F),
            faker.Random.Float(.05F, 70.0F)).Value;
        PhoneNumber? ownerPhoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("############")).Value;
        DateOnly dateOfBirth = faker.Date.PastDateOnly(30);
        HelpStatus helpStatus = HelpStatus.NeedsHelp;
        List<HelpRequisite> helpRequisites = new()
        {
            HelpRequisite.Create(
                    faker.Finance.AccountName(),
                    faker.Finance.CreditCardNumber())
                .Value,
        };

        Pet pet = new(
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