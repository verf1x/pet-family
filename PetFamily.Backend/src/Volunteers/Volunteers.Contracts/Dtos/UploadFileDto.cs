namespace Volunteers.Contracts.Dtos;

public record UploadFileDto(
    Stream Content,
    string FileName);