namespace LibraryService.Application.Subscriptions;

public record SubscriptionCheckoutResult(
    Guid SubscriptionId,
    Guid PaymentId,
    string PaymentStatus);

public record CheckoutSubscriptionRequest(
    Guid SubscriptionTypeId,
    Guid ClientId);
