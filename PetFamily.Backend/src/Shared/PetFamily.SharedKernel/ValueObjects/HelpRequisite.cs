using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects;

public class HelpRequisite : ComparableValueObject
{
    private HelpRequisite(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public string Description { get; }

    public static Result<HelpRequisite, Error> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Errors.General.ValueIsRequired(nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Errors.General.ValueIsRequired(nameof(description));
        }

        return new HelpRequisite(name, description);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Name;
        yield return Description;
    }
}