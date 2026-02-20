namespace LibraryService.Domain.Entities;

public class SubscriptionType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Period { get; set; }

    public decimal Price { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
