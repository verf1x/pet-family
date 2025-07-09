using FluentValidation;

namespace PetFamily.Application.Files.Remove;

public class RemoveFileValidator : AbstractValidator<RemoveFileRequest>
{
    public RemoveFileValidator()
    {
        RuleFor(r => r.BucketName).NotEmpty();
        
        RuleFor(r => r.ObjectName).NotEmpty();
    }
}