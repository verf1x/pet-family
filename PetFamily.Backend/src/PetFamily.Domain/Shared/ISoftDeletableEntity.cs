namespace PetFamily.Domain.Shared;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }

    DateTime? DeletionDate { get; }

    void SoftDelete();

    void Restore();
}