namespace PetFamily.Domain.ValueObjects.Pet;

public record HelpRequisites
{
    private readonly List<HelpRequisite> _values;
    
    public IReadOnlyList<HelpRequisite> Values => _values;

    // ef core ctor
    private HelpRequisites() { }
    
    public HelpRequisites(List<HelpRequisite> values)
    {
        _values = values;
    }
}