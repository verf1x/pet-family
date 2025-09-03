using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Framework;
using PetFamily.Framework.EntityIds;

namespace Species.Infrastructure.Postgres.Configurations.Write;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species.Domain.SpeciesManagement.Species>
{
    public void Configure(EntityTypeBuilder<Species.Domain.SpeciesManagement.Species> builder)
    {
        builder.ToTable("species");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => SpeciesId.Create(value));

        builder.ComplexProperty(s => s.Name, snb =>
        {
            snb.Property(n => n.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxMediumTextLength)
                .HasColumnName("name");
        });

        builder.OwnsMany(
            s => s.Breeds,
            bb =>
            {
                bb.ToJson("breeds");

                bb.Property(bp => bp.Name)
                    .IsRequired()
                    .HasMaxLength(Constants.MaxLowTextLength)
                    .HasColumnName("breed_name");

                bb.Property(br => br.Id)
                    .IsRequired()
                    .HasConversion(
                        id => id.Value,
                        value => BreedId.Create(value));
            });
    }
}