namespace PetFamily.Application.Files.Remove;

public record RemoveFileRequest(
    string BucketName,
    string ObjectName);