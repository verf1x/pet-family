using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Files;

public record FileData(Stream Stream, FilePath Path);