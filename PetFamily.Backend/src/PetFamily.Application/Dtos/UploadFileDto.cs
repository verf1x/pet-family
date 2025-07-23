namespace PetFamily.Application.Dtos;

public record UploadFileDto(
    Stream Content,
    string FileName);