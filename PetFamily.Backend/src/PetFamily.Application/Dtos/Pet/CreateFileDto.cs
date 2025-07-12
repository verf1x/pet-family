namespace PetFamily.Application.Dtos.Pet;

public record CreateFileDto(
    Stream Content,
    string FileName);