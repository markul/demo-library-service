using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface ISubscriptionCheckoutRepository
{
    Task<SubscriptionCheckoutExistingPayment?> GetByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<SubscriptionCheckoutInputData?> GetCheckoutInputDataAsync(
        Guid clientId,
        Guid subscriptionTypeId,
        CancellationToken cancellationToken);

    Task<SubscriptionCheckoutPendingResult?> CreatePendingCheckoutAsync(
        SubscriptionCheckoutPendingCreateRequest request,
        CancellationToken cancellationToken);

    Task UpdateCheckoutResultAsync(
        Guid subscriptionId,
        Guid paymentId,
        PaymentStatus paymentStatus,
        bool isSubscriptionActive,
        string? externalId,
        CancellationToken cancellationToken);
}

public sealed record SubscriptionCheckoutExistingPayment(
    Guid ClientId,
    Guid SubscriptionTypeId,
    Guid SubscriptionId,
    PaymentStatus PaymentStatus);

public sealed record SubscriptionCheckoutInputData(
    decimal BasePrice,
    string SubscriptionTypeName,
    int ClientSubscriptionsCount);

public sealed record SubscriptionCheckoutPendingCreateRequest(
    Guid ClientId,
    Guid SubscriptionTypeId,
    string IdempotencyKey,
    string SubscriptionName,
    DateTime StartDateUtc,
    decimal Amount);

public sealed record SubscriptionCheckoutPendingResult(Guid SubscriptionId, Guid PaymentId);
