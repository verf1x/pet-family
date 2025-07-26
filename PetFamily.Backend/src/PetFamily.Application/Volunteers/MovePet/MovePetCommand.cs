namespace PetFamily.Application.Volunteers.MovePet;

public record MovePetCommand(Guid VolunteerId, Guid PetId, int NewPosition);