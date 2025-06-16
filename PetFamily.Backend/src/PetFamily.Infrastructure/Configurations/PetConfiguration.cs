using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.Entities;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Species;
using PetFamily.Domain.ValueObjects.Pet;

namespace PetFamily.Infrastructure.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                p => p.Value,
                value => PetId.Create(value));

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(Constants.MaxLongTextLength);

        builder.ComplexProperty(
            p => p.SpeciesBreed,
            bsb =>
            {
                bsb.Property(p => p.BreedId)
                    .IsRequired()
                    .HasColumnName("breed_id")
                    .HasConversion(id => id.Value, value => BreedId.Create(value));
        
                bsb.Property(p => p.SpeciesId)
                    .IsRequired()
                    .HasColumnName("species_id")
                    .HasConversion(id => id.Value, value => SpeciesId.Create(value));
            });

        builder.Property(p => p.Color)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);

        builder.Property(p => p.HealthStatus)
            .IsRequired()
            .HasMaxLength(Constants.MaxMediumTextLength);

        builder.ComplexProperty(p => p.Address, pab =>
        {
            pab.Property(p => p.Locality)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            pab.Property(p => p.CountryCode)
                .IsRequired()
                .HasMaxLength(Constants.MaxCountryCodeLength);

            pab.Property(p => p.Region)
                .IsRequired(false)
                .HasMaxLength(Constants.MaxLowTextLength);

            pab.Property(p => p.PostalCode)
                .IsRequired(false)
                .HasMaxLength(Constants.MaxLowTextLength);

            pab.Property(p => p.AddressLines)
                .IsRequired()
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!)
                .HasColumnType("jsonb")
                .HasColumnName("address_lines");
        });

        builder.Property(p => p.Weight)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(p => p.Height)
            .IsRequired()
            .HasDefaultValue(0);

        builder.ComplexProperty(p => p.OwnerPhoneNumber, opn =>
        {
            opn.Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxPhoneNumberLength)
                .HasColumnName("owner_phone_number");
        });
        
        builder.Property(p => p.IsNeutered)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();
        
        builder.Property(p => p.IsVaccinated)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(p => p.HelpStatus)
            .IsRequired()
            .HasDefaultValue(HelpStatus.NeedsHelp);

        builder.OwnsOne(p => p.HelpRequisites, hdb =>
        {
            hdb.ToJson("pet_help_requisites");
            hdb.OwnsMany(hd => hd.Values, db =>
            {
                db.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(Constants.MaxLowTextLength)
                    .HasColumnName("help_requisite_name");

                db.Property(d => d.Description)
                    .IsRequired()
                    .HasMaxLength(Constants.MaxMediumTextLength)
                    .HasColumnName("help_requisite_description");
            });
        });

        builder.Property(p => p.CreatedAt)
            .IsRequired();
    }
}