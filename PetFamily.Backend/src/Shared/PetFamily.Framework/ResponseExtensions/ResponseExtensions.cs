using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework.Response;
using PetFamily.SharedKernel;

namespace PetFamily.Framework.ResponseExtensions;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Error error)
    {
        int statusCode = GetStatusCodeForErrorType(error.Type);

        Envelope envelope = Envelope.Error(error.ToErrorList());

        return new ObjectResult(envelope) { StatusCode = statusCode };
    }

    public static ActionResult ToResponse(this ErrorList errors)
    {
        if (!errors.Any())
        {
            return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
        }

        List<ErrorType> distinctErrorTypes = errors
            .Select(e => e.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeForErrorType(distinctErrorTypes.First());

        Envelope envelope = Envelope.Error(errors);

        return new ObjectResult(envelope) { StatusCode = statusCode };
    }

    private static int GetStatusCodeForErrorType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
}