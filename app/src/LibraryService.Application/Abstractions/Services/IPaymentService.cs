using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Services;

/// <summary>
/// Service for processing subscription checkout payments through external payment gateway.
/// </summary>
public interface ISubscriptionPaymentService
{
    /// <summary>
    /// Processes a payment for a subscription checkout.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="amount">The payment amount.</param>
    /// <param name="idempotencyKey">Unique key for idempotent processing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the payment processing.</returns>
    Task<SubscriptionPaymentResult> ProcessPaymentAsync(
        Guid clientId,
        Guid subscriptionId,
        decimal amount,
        string idempotencyKey,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents the result of a subscription payment processing operation.
/// </summary>
/// <param name="IsSuccess">Whether the payment was successful.</param>
/// <param name="ExternalId">The external payment ID from the payment service (if successful).</param>
/// <param name="ErrorMessage">Error message if the payment failed.</param>
public record SubscriptionPaymentResult(
    bool IsSuccess,
    string? ExternalId = null,
    string? ErrorMessage = null);
