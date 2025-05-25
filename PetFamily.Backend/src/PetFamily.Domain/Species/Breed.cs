using PetFamily.Domain.Shared;

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
            return "Breed name cannot be empty";
        
        return new Breed(name);
    }
}