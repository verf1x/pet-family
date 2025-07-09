using FluentValidation;

namespace PetFamily.Application.Files.Upload;

public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
{
    public UploadFileRequestValidator()
    {
        RuleFor(r => r.Stream.Length).GreaterThan(0);

        RuleFor(r => r.BucketName).NotEmpty();
        
        RuleFor(r => r.ObjectName).NotEmpty();
    }
}