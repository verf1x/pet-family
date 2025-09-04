namespace PetFamily.SharedKernel;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }

    DateTime? DeletionDate { get; }

    void SoftDelete();

    void Restore();
}