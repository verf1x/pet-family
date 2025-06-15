using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.ValueObjects.Pet;

public record HelpRequisites
{
    private readonly List<HelpDetail> _values = [];
    
    public IReadOnlyList<HelpDetail> Values => _values;
    
    public Result<Error> AddHelpDetails(HelpDetail detail)
    {
        if (_values.Contains(detail))
            return Errors.General.Conflict();
        
        _values.Add(detail);
        
        return Result.Success<Error>(null!);
    }
}