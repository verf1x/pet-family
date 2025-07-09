using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Files.GetPresignedUrl;
using PetFamily.Application.Files.Remove;
using PetFamily.Application.Files.Upload;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.Create;
using PetFamily.Application.Volunteers.Delete;
using PetFamily.Application.Volunteers.UpdateMainInfo;

namespace PetFamily.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateMainInfoHandler>();
        services.AddScoped<HardDeleteVolunteerHandler>();
        services.AddScoped<SoftDeleteVolunteerHandler>();
        services.AddScoped<AddPetHandler>();
        services.AddScoped<UploadFileHandler>();
        services.AddScoped<RemoveFileHandler>();
        services.AddScoped<GetPresignedUrlHandler>();
        
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        
        return services;
    }
} 