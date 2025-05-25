using PetFamily.Domain.Shared;

namespace PetFamily.Domain.ValueObjects.Pet;

public class HelpDetails
{
    private readonly List<HelpDetail> _details = [];
    
    public IReadOnlyList<HelpDetail> Details => _details;
    
    public Result AddHelpDetails(HelpDetail detail)
    {
        if(_details.Contains(detail))
            return "Details already exists in the list.";
        
        _details.Add(detail);
        
        return Result.Success();
    }
}