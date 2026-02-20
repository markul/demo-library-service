namespace LibraryService.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }

    public string UniqueId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public Guid SubscriptionId { get; set; }

    public Guid ClientId { get; set; }

    public string? ExternalId { get; set; }

    public PaymentStatus Status { get; set; }

    public Subscription? Subscription { get; set; }

    public Client? Client { get; set; }
}
