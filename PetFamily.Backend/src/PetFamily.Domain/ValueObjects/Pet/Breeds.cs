using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Species;

namespace PetFamily.Domain.ValueObjects.Pet;

public record Breeds
{
    private readonly List<Breed> _values = [];
    
    public IReadOnlyList<Breed> Values => _values;
    
    public UnitResult<Error> Add(Breed breed)
    {
        if(_values.Contains(breed))
            return Errors.General.Conflict(breed.Id.Value);
        
        _values.Add(breed);
        
        return UnitResult.Success<Error>();
    }
}