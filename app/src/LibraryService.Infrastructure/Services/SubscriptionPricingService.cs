using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Abstractions.Services;

namespace LibraryService.Infrastructure.Services;

/// <summary>
/// Implementation of subscription pricing service for calculating subscription prices.
/// Applies discount rules based on client's existing subscription count.
/// </summary>
public class SubscriptionPricingService : ISubscriptionPricingService
{
    private readonly ISubscriptionTypeRepository _subscriptionTypeRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    /// <summary>
    /// Discount percentage applied for each existing subscription (5%).
    /// </summary>
    private const decimal DiscountPerSubscription = 0.05m;

    /// <summary>
    /// Maximum discount percentage (20%).
    /// </summary>
    private const decimal MaxDiscount = 0.20m;

    public SubscriptionPricingService(
        ISubscriptionTypeRepository subscriptionTypeRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionTypeRepository = subscriptionTypeRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<decimal> CalculatePriceAsync(
        Guid subscriptionTypeId,
        Guid clientId,
        CancellationToken cancellationToken)
    {
        // Get the subscription type to retrieve base price
        var subscriptionType = await _subscriptionTypeRepository.GetByIdAsync(subscriptionTypeId, cancellationToken);
        if (subscriptionType is null)
        {
            throw new InvalidOperationException($"Subscription type with ID {subscriptionTypeId} not found.");
        }

        var basePrice = subscriptionType.Price;

        // Count client's existing subscriptions for discount calculation
        var existingSubscriptionCount = await _subscriptionRepository.CountClientSubscriptionsAsync(clientId, cancellationToken);

        // Calculate discount: 5% per subscription, max 20%
        var discountPercentage = Math.Min(existingSubscriptionCount * DiscountPerSubscription, MaxDiscount);
        var discountAmount = basePrice * discountPercentage;
        var finalPrice = basePrice - discountAmount;

        return finalPrice;
    }
}
