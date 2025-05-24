using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Species;

public class Species
{
    private readonly List<Breed> _breeds = [];
    
    public SpeciesId Id { get; private set; }
    public IReadOnlyList<Breed> Breeds => _breeds;

    private Species()
    {
        Id = SpeciesId.CreateNew();
    }

    public static Species Create()
    {
        return new Species();
    }
    
    public Result AddBreed(Breed breed)
    {
        if(_breeds.Contains(breed))
            return Result.Failure("Breed already exists in this species.");
        
        _breeds.Add(breed);
        
        return Result.Success();
    }
}