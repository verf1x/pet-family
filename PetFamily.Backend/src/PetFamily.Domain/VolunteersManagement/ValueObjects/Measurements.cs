using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

public record Measurements
{
    public float Height { get; }
    
    public float Weight { get; }
    
    private Measurements(float height, float weight)
    {
        Height = height;
        Weight = weight;
    }
    
    public static Result<Measurements, Error> Create(float height, float weight)
    {
        if (height <= 0)
            return Errors.General.ValueIsInvalid(nameof(height));
        
        if (weight <= 0)
            return Errors.General.ValueIsInvalid(nameof(weight));
        
        return new Measurements(height, weight);
    }
}