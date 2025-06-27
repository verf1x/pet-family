namespace PetFamily.Domain.Shared;

public record Error
{
    private const string Separator = "||";
    
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }
    
    public static Error Validation(string code, string message) => new Error(code, message, ErrorType.Validation);
    
    public static Error NotFound(string code, string message) => new Error(code, message, ErrorType.NotFound);
    
    public static Error Failure(string code, string message) => new Error(code, message, ErrorType.Failure);
    
    public static Error Conflict(string code, string message) => new Error(code, message, ErrorType.Conflict);

    public string Serialize()
    {
        return string.Join(Separator, Code, Message, Type);
    }

    public static Error Deserialize(string serialized)
    {
        var parts = serialized.Split(Separator);
        
        if(parts.Length < 2)
            throw new ArgumentException("Invalid serialized format");
        
        if(Enum.TryParse<ErrorType>(parts[2], out var type) is false)
            throw new ArgumentException("Invalid serialized format");
        
        return new Error(parts[0], parts[1], type);
    }
}

public enum ErrorType
{
    Validation,
    NotFound,
    Failure,
    Conflict,
}
