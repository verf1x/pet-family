using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.SetMainPetPhoto;

public record SetMainPetPhotoCommand(Guid VolunteerId, Guid PetId, string PhotoPath) : ICommand;