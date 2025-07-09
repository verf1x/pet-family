namespace PetFamily.Application.FileProvider;

public record GetPresignedUrlData(
    string BucketName,
    string ObjectName,
    TimeSpan ExpirationTime);