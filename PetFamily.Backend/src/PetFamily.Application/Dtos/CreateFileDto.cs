namespace PetFamily.Application.Dtos;

public record CreateFileDto(
    Stream Content,
    string FileName);