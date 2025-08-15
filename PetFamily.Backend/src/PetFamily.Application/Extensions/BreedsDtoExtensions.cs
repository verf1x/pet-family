using PetFamily.Contracts.Dtos.Species;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.Extensions;

public static class BreedsDtoExtensions
{
    public static List<Breed> ToBreedsCollection(this IEnumerable<string> breedsDto)
    {
        return breedsDto
            .Select(b => Breed.Create(b).Value)
            .ToList();
    }
}