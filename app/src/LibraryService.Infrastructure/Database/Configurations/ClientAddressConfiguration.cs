using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryService.Infrastructure.Database.Configurations;

public class ClientAddressConfiguration : IEntityTypeConfiguration<ClientAddress>
{
    public void Configure(EntityTypeBuilder<ClientAddress> builder)
    {
        builder.ToTable("ClientAddresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.ClientAddresses)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
