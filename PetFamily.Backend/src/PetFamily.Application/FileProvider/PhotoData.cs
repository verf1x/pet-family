using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.FileProvider;
    
public record PhotoData(Stream Stream, PhotoPath Path);