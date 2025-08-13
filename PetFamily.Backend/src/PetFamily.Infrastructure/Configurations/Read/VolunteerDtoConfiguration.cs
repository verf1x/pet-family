using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Contracts.Dtos.Volunteer;

namespace PetFamily.Infrastructure.Configurations.Read;

public class VolunteerDtoConfiguration : IEntityTypeConfiguration<VolunteerDto>
{
    public void Configure(EntityTypeBuilder<VolunteerDto> builder)
    {
        builder.ToTable("volunteers");
        
        builder.HasKey(v => v.Id);

        builder.HasMany<PetDto>(v => v.Pets)
            .WithOne()
            .HasForeignKey(p => p.VolunteerId);
    }
}