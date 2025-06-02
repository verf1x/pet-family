using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.Entities;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Infrastructure.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.Id)
            .HasConversion(
                id => id.Value,
                value => VolunteerId.Create(value));

        builder.ComplexProperty(v => v.FullName, b =>
        {
            b.Property(fn => fn.FirstName)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength)
                .HasColumnName("full_name");
            b.Property(fn => fn.LastName)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength)
                .HasColumnName("last_name");
            b.Property(fn => fn.MiddleName)
                .IsRequired(false)
                .HasMaxLength(Constants.MaxLowTextLength)
                .HasColumnName("middle_name");
        });

        builder.Property(v => v.Email)
            .IsRequired()
            .HasMaxLength(Constants.MaxEmailLength);
        
        builder.Property(v => v.Description)
            .IsRequired()
            .HasMaxLength(Constants.MaxLongTextLength);
        
        builder.Property(v => v.ExperienceYears)
            .IsRequired()
            .HasDefaultValue(0);

        builder.ComplexProperty(
            v => v.PhoneNumber,
            b =>
        {
            b.Property(pn => pn.Number)
                .IsRequired()
                .HasMaxLength(Constants.MaxPhoneNumberLength)
                .HasColumnName("phone_number");
        });

        builder.OwnsOne(
            v => v.SocialNetworks,
            b =>
        {
            b.ToJson("social_networks");
            b.OwnsMany(snb => snb.Values, sn =>
            {
                sn.Property(snn => snn.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(Constants.MaxLowTextLength);

                sn.Property(snn => snn.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasMaxLength(Constants.MaxUrlLength);
            });
        });

        builder.ComplexProperty(
            v => v.HelpDetail,
            hdb =>
            {
                hdb.Property(hd => hd.Name)
                    .IsRequired()
                    .HasMaxLength(Constants.MaxLowTextLength);

                hdb.Property(hd => hd.Description)
                    .IsRequired()
                    .HasMaxLength(Constants.MaxMediumTextLength);
            });

        builder.HasMany(v => v.AllPets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}