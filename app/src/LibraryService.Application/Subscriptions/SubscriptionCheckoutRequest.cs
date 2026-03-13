namespace LibraryService.Application.Subscriptions;

public record SubscriptionCheckoutRequest(
    Guid ClientId,
    Guid SubscriptionTypeId,
    string IdempotencyKey);

public record SubscriptionCheckoutResult(
    Guid SubscriptionId,
    string PaymentStatus);

public record PriceCalculationResult(
    decimal BasePrice,
    decimal FinalPrice,
    int DiscountPercent);

public record PriceCalculationInput(
    Guid SubscriptionTypeId,
    Guid ClientId);
