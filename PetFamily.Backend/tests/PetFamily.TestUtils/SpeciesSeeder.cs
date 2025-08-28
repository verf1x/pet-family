using Bogus;
using PetFamily.Application.SpeciesManagement;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.SpeciesManagement;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.TestUtils;

public static class SpeciesSeeder
{
    public static async Task<Species> SeedSpeciesAsync(ISpeciesRepository repository, WriteDbContext dbContext)
    {
        var faker = new Faker();

        var speciesId = SpeciesId.CreateNew();
        string name = faker.Lorem.Word();

        var species = new Species(
            speciesId,
            Name.Create(name).Value);

        await repository.AddAsync(species);
        await dbContext.SaveChangesAsync();

        return species;
    }

    public static async Task<Species> SeedSpeciesWithBreedsAsync(
        ISpeciesRepository repository,
        WriteDbContext dbContext)
    {
        var faker = new Faker();

        var speciesId = SpeciesId.CreateNew();
        string name = faker.Lorem.Word();

        var species = new Species(
            speciesId,
            Name.Create(name).Value);

        species.AddBreeds([
            Breed.Create(faker.Lorem.Word()).Value,
            Breed.Create(faker.Lorem.Word()).Value,
        ]);

        await repository.AddAsync(species);
        await dbContext.SaveChangesAsync();

        return species;
    }
}