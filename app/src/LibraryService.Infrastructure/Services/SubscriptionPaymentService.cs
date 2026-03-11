using LibraryService.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace LibraryService.Infrastructure.Services;

/// <summary>
/// Implementation of subscription payment service for processing payments through external payment gateway.
/// This is a placeholder implementation that simulates successful payments.
/// </summary>
public class SubscriptionPaymentService : ISubscriptionPaymentService
{
    private readonly ILogger<SubscriptionPaymentService> _logger;

    public SubscriptionPaymentService(ILogger<SubscriptionPaymentService> logger)
    {
        _logger = logger;
    }

    public async Task<SubscriptionPaymentResult> ProcessPaymentAsync(
        Guid clientId,
        Guid subscriptionId,
        decimal amount,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing subscription payment for ClientId: {ClientId}, SubscriptionId: {SubscriptionId}, Amount: {Amount}, IdempotencyKey: {IdempotencyKey}",
            clientId, subscriptionId, amount, idempotencyKey);

        // Simulate async payment processing
        await Task.Delay(100, cancellationToken);

        // Generate a mock external payment ID
        var externalId = $"pay_{Guid.NewGuid():N}";

        _logger.LogInformation(
            "Subscription payment processed successfully. ExternalId: {ExternalId}",
            externalId);

        return new SubscriptionPaymentResult(
            IsSuccess: true,
            ExternalId: externalId);
    }
}
