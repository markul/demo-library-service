using System.ComponentModel.DataAnnotations;

namespace LibraryService.Application.Subscriptions;

public sealed class CheckoutSubscriptionRequest
{
    [Required]
    public Guid ClientId { get; init; }

    [Required]
    public Guid SubscriptionTypeId { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 1)]
    public string IdempotencyKey { get; init; } = string.Empty;
}

public sealed record CheckoutSubscriptionResponse(Guid SubscriptionId, string PaymentStatus);
