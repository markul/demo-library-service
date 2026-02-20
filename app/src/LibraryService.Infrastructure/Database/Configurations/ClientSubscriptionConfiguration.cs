using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryService.Infrastructure.Database.Configurations;

public class ClientSubscriptionConfiguration : IEntityTypeConfiguration<ClientSubscription>
{
    public void Configure(EntityTypeBuilder<ClientSubscription> builder)
    {
        builder.ToTable("client_subscriptions");

        builder.HasKey(x => new { x.ClientId, x.SubscriptionId });

        builder.Property(x => x.ClientId)
            .HasColumnName("client_id");

        builder.Property(x => x.SubscriptionId)
            .HasColumnName("subscription_id");

        builder.HasOne(x => x.Client)
            .WithMany(x => x.ClientSubscriptions)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Subscription)
            .WithMany(x => x.ClientSubscriptions)
            .HasForeignKey(x => x.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SubscriptionId);
    }
}
