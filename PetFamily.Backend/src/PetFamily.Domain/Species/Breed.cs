using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Species;

public class Breed
{
    public BreedId Id { get; private set; }
    public string Name { get; private set; }

    private Breed(string name)
    {
        Id = BreedId.CreateNew();
        Name = name;
    }
    
    public static Result<Breed> Create(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Result.Failure<Breed>("Breed name cannot be empty");
        
        Breed breed = new(name);
        
        return Result.Success(breed);
    }
}