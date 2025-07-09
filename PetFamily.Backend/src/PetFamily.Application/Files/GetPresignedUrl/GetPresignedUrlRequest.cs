namespace PetFamily.Application.Files.GetPresignedUrl;

public record GetPresignedUrlRequest(
    string BucketName,
    string ObjectName,
    TimeSpan ExpirationTime);