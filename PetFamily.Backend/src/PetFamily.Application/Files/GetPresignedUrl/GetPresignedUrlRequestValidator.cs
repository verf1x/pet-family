using FluentValidation;

namespace PetFamily.Application.Files.GetPresignedUrl;

public class GetPresignedUrlRequestValidator : AbstractValidator<GetPresignedUrlRequest>
{
    public GetPresignedUrlRequestValidator()
    {
        RuleFor(r => r.BucketName).NotEmpty();
        
        RuleFor(r => r.ObjectName).NotEmpty();

        RuleFor(r => r.ExpirationTime).GreaterThan(TimeSpan.Zero);
    }
}