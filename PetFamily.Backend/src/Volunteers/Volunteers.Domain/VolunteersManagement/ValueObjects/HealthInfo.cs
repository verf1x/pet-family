using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class HealthInfo : ComparableValueObject
{
    private HealthInfo(string healthStatus, bool isNeutered, bool isVaccinated)
    {
        HealthStatus = healthStatus;
        IsNeutered = isNeutered;
        IsVaccinated = isVaccinated;
    }

    public string HealthStatus { get; }

    public bool IsNeutered { get; }

    public bool IsVaccinated { get; }

    public static Result<HealthInfo, Error> Create(string healthStatus, bool isNeutered, bool isVaccinated)
    {
        if (string.IsNullOrWhiteSpace(healthStatus))
        {
            return Errors.General.ValueIsRequired(nameof(healthStatus));
        }

        return new HealthInfo(healthStatus, isNeutered, isVaccinated);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return HealthStatus;
        yield return IsNeutered;
        yield return IsVaccinated;
    }
}