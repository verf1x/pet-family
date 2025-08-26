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
        var name = faker.Lorem.Word();

        var breeds = new List<Breed>
        {
            Breed.Create(faker.Lorem.Word()).Value,
            Breed.Create(faker.Lorem.Word()).Value,
        };

        var species = new Species(
            speciesId,
            Name.Create(name).Value);

        species.AddBreeds(breeds);

        await repository.AddAsync(species);
        await dbContext.SaveChangesAsync();

        return species;
    }
}