namespace PetFamily.SharedKernel;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            string label = name ?? "value";
            return Error.Validation("value.is.invalid", $"{label} is invalid", label);
        }

        public static Error NotFound(Guid? id = null)
        {
            string forId = id is null ? string.Empty : $" for id '{id}'";
            return Error.NotFound("record.not.found", $"record not found{forId}");
        }

        public static Error ValueIsRequired(string? name = null)
        {
            string label = name is null ? " " : " " + name + " ";
            return Error.NotFound("value.is.required", $"invalid{label}length");
        }

        public static Error Conflict(Guid? id = null)
        {
            string forId = id is null ? string.Empty : $" for id '{id}'";
            return Error.NotFound("value.already.exists", $"value already exists{forId}");
        }
    }

    public static class Volunteer
    {
        public static Error AlreadyExists() =>
            Error.Validation("record.already.exists", "Volunteer already exists", null);
    }

    public static class Species
    {
        public static Error AlreadyExists() =>
            Error.Validation("record.already.exists", "Species already exists", null);
    }

    public static class User
    {
        public static Error InvalidCredentials() =>
            Error.Validation("credentials.is.invalid", $"Your credentials are invalid", null);
    }
}