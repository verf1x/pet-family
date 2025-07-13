using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

public record FilePath
{
    public string Path { get; }
    
    private FilePath(string path)
    {
        Path = path;
    }

    public static Result<FilePath, Error> Create(Guid value, string extension)
    {
        //TODO: валидация
        var fullPath = value + "." + extension;
        
        return new FilePath(fullPath);
    }

    public static Result<FilePath, Error> Create(string fullPath)
    {
        return new FilePath(fullPath);
    }
}