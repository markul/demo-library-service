namespace LibraryService.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime RegisteredAtUtc { get; set; }

    public ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
