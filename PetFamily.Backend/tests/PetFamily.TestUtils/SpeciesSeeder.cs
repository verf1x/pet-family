using Bogus;
using PetFamily.Framework.EntityIds;
using Species.Application.SpeciesManagement;
using Species.Domain.SpeciesManagement.ValueObjects;
using Species.Infrastructure.Postgres.DbContexts;

namespace PetFamily.TestUtils;

public static class SpeciesSeeder
{
    public static async Task<Species.Domain.SpeciesManagement.Species> SeedSpeciesAsync(ISpeciesRepository repository, SpeciesWriteDbContext dbContext)
    {
        var faker = new Faker();

        var speciesId = SpeciesId.CreateNew();
        string name = faker.Lorem.Word();

        var species = new Species.Domain.SpeciesManagement.Species(
            speciesId,
            Name.Create(name).Value);

        await repository.AddAsync(species);
        await dbContext.SaveChangesAsync();

        return species;
    }

    public static async Task<Species.Domain.SpeciesManagement.Species> SeedSpeciesWithBreedsAsync(
        ISpeciesRepository repository,
        SpeciesWriteDbContext dbContext)
    {
        var faker = new Faker();

        var speciesId = SpeciesId.CreateNew();
        string name = faker.Lorem.Word();

        var species = new Species.Domain.SpeciesManagement.Species(
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