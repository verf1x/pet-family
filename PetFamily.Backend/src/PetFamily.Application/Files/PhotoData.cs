using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Files;
    
public record PhotoData(Stream Stream, PhotoPath Path);