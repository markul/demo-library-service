namespace LibraryService.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid SubscriptionTypeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime StartDateUtc { get; set; }

    public SubscriptionType? SubscriptionType { get; set; }

    public ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
