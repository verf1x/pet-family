namespace PetFamily.Application.Files.Upload;

public record UploadFileRequest(
    Stream Stream,
    string BucketName,
    string ObjectName);