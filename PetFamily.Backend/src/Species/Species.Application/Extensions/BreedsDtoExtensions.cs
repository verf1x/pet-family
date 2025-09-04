using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Application.Extensions;

public static class BreedsDtoExtensions
{
    public static List<Breed> ToBreedsCollection(this IEnumerable<string> breedsDto) =>
        breedsDto
            .Select(b => Breed.Create(b).Value)
            .ToList();
}