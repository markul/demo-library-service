namespace LibraryService.Domain.Entities;

public class ClientSubscription
{
    public Guid ClientId { get; set; }

    public Guid SubscriptionId { get; set; }

    public Client? Client { get; set; }

    public Subscription? Subscription { get; set; }
}
