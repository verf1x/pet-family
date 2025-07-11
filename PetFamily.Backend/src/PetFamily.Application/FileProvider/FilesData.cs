namespace PetFamily.Application.FileProvider;

public record FilesData(IEnumerable<FileContent> Files, string BucketName);
    
public record FileContent(Stream Stream, string ObjectName);