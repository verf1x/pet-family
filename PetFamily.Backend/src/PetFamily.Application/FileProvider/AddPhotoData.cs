using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.FileProvider;
    
public record AddPhotoData(Stream Stream, PhotoPath PhotoPath);