using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Application.FileProvider;
    
public record FileData(Stream Stream, FilePath FilePath, string BucketName);