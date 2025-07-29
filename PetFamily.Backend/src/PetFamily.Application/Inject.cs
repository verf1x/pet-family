using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;
using PetFamily.Application.VolunteersManagement.UseCases.AddPet;
using PetFamily.Application.VolunteersManagement.UseCases.Create;
using PetFamily.Application.VolunteersManagement.UseCases.Delete;
using PetFamily.Application.VolunteersManagement.UseCases.MovePet;
using PetFamily.Application.VolunteersManagement.UseCases.RemovePetPhotos;
using PetFamily.Application.VolunteersManagement.UseCases.UpdateMainInfo;
using PetFamily.Application.VolunteersManagement.UseCases.UploadPetPhotos;

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
        services.AddScoped<UploadPetPhotosHandler>();
        services.AddScoped<RemovePetPhotosHandler>();
        services.AddScoped<MovePetHandler>();
        services.AddScoped<GetPetsWithPaginationHandler>();
        
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);
        
        return services;
    }
}