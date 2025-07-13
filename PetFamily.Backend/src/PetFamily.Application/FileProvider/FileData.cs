using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.FileProvider;
    
public record FileData(Stream Stream, FilePath FilePath, string BucketName);