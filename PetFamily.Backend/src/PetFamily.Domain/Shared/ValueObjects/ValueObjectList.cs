using System.Collections;

namespace PetFamily.Domain.Shared.ValueObjects;

public class ValueObjectList<T> : IReadOnlyList<T>
{
    public IReadOnlyList<T> Values { get; } = null!;
    
    public T this[int index] => Values[index];
    
    public int Count => Values.Count;
    
    public ValueObjectList() { }

    public ValueObjectList(IEnumerable<T> list)
    {
        Values = new List<T>(list);
    }
    
    public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public static implicit operator ValueObjectList<T>(List<T> list) 
        => new(list);

    public static implicit operator List<T>(ValueObjectList<T> list)
        => list.Values.ToList();
}