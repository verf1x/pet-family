using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;
using Volunteers.Contracts.Dtos;

namespace Volunteers.Application.Dtos.Validators;

public class UploadFileDtoValidator : AbstractValidator<UploadFileDto>
{
    public UploadFileDtoValidator()
    {
        RuleFor(v => v.FileName)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        RuleFor(v => v.Content)
            .Must(c => c.Length is > 0 and < 10485760)
            .WithError(Error.Validation(
                "validation.fileSize",
                "File size must be between 1 byte and 10 MB",
                nameof(UploadFileDto.Content.Length)));
    }
}