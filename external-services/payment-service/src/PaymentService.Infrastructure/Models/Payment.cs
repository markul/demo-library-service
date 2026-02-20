namespace PaymentService.Infrastructure.Models;

public sealed class Payment
{
    public Guid Id { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string? Description { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Accepted;

    public DateTime CreatedAtUtc { get; set; }
}
