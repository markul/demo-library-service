namespace LibraryService.Application.Abstractions.Services;

public interface ISubscriptionCheckoutPaymentGateway
{
    Task<SubscriptionCheckoutPaymentResult> CreatePaymentAsync(
        SubscriptionCheckoutPaymentRequest request,
        CancellationToken cancellationToken);
}

public sealed record SubscriptionCheckoutPaymentRequest(
    Guid ClientId,
    Guid SubscriptionId,
    Guid SubscriptionTypeId,
    decimal Amount,
    string IdempotencyKey);

public sealed record SubscriptionCheckoutPaymentResult(
    SubscriptionCheckoutPaymentStatus Status,
    string? ExternalId);

public enum SubscriptionCheckoutPaymentStatus
{
    Accepted = 1,
    Rejected = 2,
    TechnicalFailure = 3,
}
