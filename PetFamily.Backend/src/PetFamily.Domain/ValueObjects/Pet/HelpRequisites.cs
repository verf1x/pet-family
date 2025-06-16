using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.ValueObjects.Pet;

public record HelpRequisites
{
    private readonly List<HelpRequisite> _values;
    
    public IReadOnlyList<HelpRequisite> Values => _values;

    // ef core ctor
    private HelpRequisites() {}
    
    private HelpRequisites(List<HelpRequisite> values)
    {
        _values = values;
    }
    
    public static Result<HelpRequisites, Error> Create(IEnumerable<HelpRequisite> helpRequisites)
    {
        return Result.Success<HelpRequisites, Error>(new HelpRequisites(helpRequisites.ToList()));
    }
}