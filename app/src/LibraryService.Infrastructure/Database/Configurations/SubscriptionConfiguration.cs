using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryService.Infrastructure.Database.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SubscriptionTypeId)
            .HasColumnName("subscription_type_id")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.StartDateUtc)
            .HasColumnName("start_date_utc")
            .IsRequired();

        builder.HasOne(x => x.SubscriptionType)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.SubscriptionTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
