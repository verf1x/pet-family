using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Species;

namespace PetFamily.Domain.ValueObjects.Pet;

public record Breeds
{
    private readonly List<Breed> _values = [];
    
    public IReadOnlyList<Breed> Values => _values;
    
    public Result<Error> AddBreed(Breed breed)
    {
        if(_values.Contains(breed))
            return Errors.General.Conflict(breed.Id.Value);
        
        _values.Add(breed);
        
        return Result.Success<Error>(null!);
    }
}