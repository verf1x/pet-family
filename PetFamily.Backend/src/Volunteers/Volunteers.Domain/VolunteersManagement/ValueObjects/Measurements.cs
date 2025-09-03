using CSharpFunctionalExtensions;
using PetFamily.Framework;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class Measurements : ComparableValueObject
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

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Height;
        yield return Weight;
    }
}