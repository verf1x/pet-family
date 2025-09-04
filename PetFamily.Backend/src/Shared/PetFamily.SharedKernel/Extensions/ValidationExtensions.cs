using FluentValidation.Results;

namespace PetFamily.SharedKernel.Extensions;

public static class ValidationExtensions
{
    public static ErrorList ToErrorList(this ValidationResult validationResult)
    {
        IEnumerable<Error> errors =
            from validationError in validationResult.Errors
            let errorMessage = validationError.ErrorMessage
            let error = Error.Deserialize(errorMessage)
            select Error.Validation(error.Code, error.Message, error.InvalidField);

        return errors.ToList();
    }
}