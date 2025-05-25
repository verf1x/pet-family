using PetFamily.Domain.Shared;
using PetFamily.Domain.Species;

namespace PetFamily.Domain.ValueObjects.Pet;

public class Breeds
{
    private readonly List<Breed> _allBreeds = [];
    
    public IReadOnlyList<Breed> AllBreeds => _allBreeds;
    
    public Result AddBreed(Breed breed)
    {
        if(_allBreeds.Contains(breed))
            return "Breed already exists in this species.";
        
        _allBreeds.Add(breed);
        
        return Result.Success();
    }
}