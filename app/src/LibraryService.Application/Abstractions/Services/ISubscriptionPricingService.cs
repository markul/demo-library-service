namespace LibraryService.Application.Abstractions.Services;

/// <summary>
/// Service for calculating subscription prices.
/// </summary>
public interface ISubscriptionPricingService
{
    /// <summary>
    /// Calculates the final price for a subscription based on the subscription type and client's subscription history.
    /// </summary>
    /// <param name="subscriptionTypeId">The subscription type identifier.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The calculated price for the subscription.</returns>
    Task<decimal> CalculatePriceAsync(
        Guid subscriptionTypeId,
        Guid clientId,
        CancellationToken cancellationToken);
}
