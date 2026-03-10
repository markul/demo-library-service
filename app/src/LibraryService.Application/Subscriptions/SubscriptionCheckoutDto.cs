using LibraryService.Domain.Entities;

namespace LibraryService.Application.Subscriptions;

public record SubscriptionCheckoutResult(
    Guid SubscriptionId,
    PaymentStatus PaymentStatus);
