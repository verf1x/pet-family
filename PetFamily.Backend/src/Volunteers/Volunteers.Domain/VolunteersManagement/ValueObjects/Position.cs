using CSharpFunctionalExtensions;
using PetFamily.Framework;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class Position : ComparableValueObject
{
    public static Position First => new(1);

    public int Value { get; }

    private Position(int value)
    {
        Value = value;
    }

    public static Result<Position, Error> Create(int value)
    {
        if (value < 1)
            return Errors.General.ValueIsInvalid(nameof(value));

        return new Position(value);
    }

    public static implicit operator int(Position position) => position.Value;

    public Result<Position, Error> Forward()
        => Create(Value + 1);

    public Result<Position, Error> Backward()
        => Create(Value - 1);

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}