using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.VolunteersManagement.Entities;
using PetFamily.Domain.VolunteersManagement.Enums;
using PetFamily.Domain.VolunteersManagement.ValueObjects;
using PetFamily.Infrastructure.Extensions;

namespace PetFamily.Infrastructure.Configurations.Write;

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

        builder.ComplexProperty(
            p => p.Nickname,
            nb =>
            {
                nb.Property(n => n.Value)
                    .IsRequired()
                    .HasMaxLength(Domain.Shared.Constants.MaxLowTextLength)
                    .HasColumnName("nickname");
            });

        builder.ComplexProperty(
            p => p.Description,
            db =>
            {
                db.Property(d => d.Value)
                    .IsRequired()
                    .HasMaxLength(Domain.Shared.Constants.MaxLongTextLength)
                    .HasColumnName("description");
            });

        builder.ComplexProperty(
            p => p.Position,
            snb =>
            {
                snb.Property(sn => sn.Value)
                    .IsRequired()
                    .HasColumnName("position");
            });

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

        builder.ComplexProperty(
            p => p.Color,
            cb =>
            {
                cb.Property(c => c.Value)
                    .IsRequired()
                    .HasMaxLength(Domain.Shared.Constants.MaxLowTextLength)
                    .HasColumnName("color");
            });

        builder.ComplexProperty(
            p => p.HealthInfo,
            hb =>
            {
                hb.Property(h => h.HealthStatus)
                    .IsRequired()
                    .HasMaxLength(Domain.Shared.Constants.MaxMediumTextLength)
                    .HasColumnName("health_status");

                hb.Property(h => h.IsNeutered)
                    .IsRequired()
                    .HasColumnName("is_neutered");

                hb.Property(h => h.IsVaccinated)
                    .IsRequired()
                    .HasColumnName("is_vaccinated");
            });

        builder.ComplexProperty(
            p => p.Address,
            pab =>
            {
                pab.Property(p => p.Locality)
                    .IsRequired()
                    .HasColumnName("locality")
                    .HasMaxLength(Domain.Shared.Constants.MaxLowTextLength);

                pab.Property(p => p.CountryCode)
                    .IsRequired()
                    .HasColumnName("country_code")
                    .HasMaxLength(Domain.Shared.Constants.MaxCountryCodeLength);

                pab.Property(p => p.Region)
                    .IsRequired(false)
                    .HasColumnName("region")
                    .HasMaxLength(Domain.Shared.Constants.MaxLowTextLength);

                pab.Property(p => p.PostalCode)
                    .IsRequired(false)
                    .HasColumnName("postal_code")
                    .HasMaxLength(Domain.Shared.Constants.MaxLowTextLength);

                pab.Property(p => p.AddressLines)
                    .IsRequired()
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!)
                    .HasColumnType("jsonb")
                    .HasColumnName("address_lines");
            });

        builder.ComplexProperty(
            p => p.Measurements,
            mb =>
            {
                mb.Property(m => m.Height)
                    .IsRequired()
                    .HasColumnName("height")
                    .HasDefaultValue(0.0f);

                mb.Property(m => m.Weight)
                    .IsRequired()
                    .HasColumnName("weight")
                    .HasDefaultValue(0.0f);
            });

        builder.ComplexProperty(
            p => p.OwnerPhoneNumber,
            opn =>
            {
                opn.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(Domain.Shared.Constants.MaxPhoneNumberLength)
                    .HasColumnName("owner_phone_number");
            });

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Property(p => p.HelpStatus)
            .IsRequired()
            .HasDefaultValue(HelpStatus.NeedsHelp);

        builder.OwnsMany(
            p => p.HelpRequisites,
            hdb =>
            {
                hdb.ToJson("help_requisites");
                hdb.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(Domain.Shared.Constants.MaxLowTextLength)
                    .HasColumnName("help_requisite_name");

                hdb.Property(d => d.Description)
                    .IsRequired()
                    .HasMaxLength(Domain.Shared.Constants.MaxMediumTextLength)
                    .HasColumnName("help_requisite_description");
            });

        builder.Property(p => p.Photos)
            .HasValueObjectCollectionJsonConversion(
                file => new PetFileDto { FilePath = file.Path },
                dto => new Photo(dto.FilePath))
            .HasColumnName("photos");

        builder.OwnsOne(p => p.MainPhoto, vo =>
        {
            vo.Property(v => v.Path)
                .HasColumnName("main_photo_path")
                .IsRequired(false)
                .HasMaxLength(Domain.Shared.Constants.MaxLongTextLength);
        });

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property<bool>("IsDeleted")
            .HasColumnName("is_deleted");

        builder.Property<DateTime?>("DeletionDate")
            .HasColumnName("deletion_date");
    }
}