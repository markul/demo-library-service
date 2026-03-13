using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Abstractions.Services;
using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Services;

public class PriceCalculator : IPriceCalculator
{
    private readonly ISubscriptionTypeRepository _subscriptionTypeRepository;
    private readonly IClientRepository _clientRepository;

    public PriceCalculator(ISubscriptionTypeRepository subscriptionTypeRepository, IClientRepository clientRepository)
    {
        _subscriptionTypeRepository = subscriptionTypeRepository;
        _clientRepository = clientRepository;
    }

    public async Task<PriceCalculationResult> CalculatePriceAsync(PriceCalculationInput input, CancellationToken cancellationToken)
    {
        var subscriptionType = await _subscriptionTypeRepository.GetByIdAsync(input.SubscriptionTypeId, cancellationToken);
        if (subscriptionType is null)
        {
            throw new InvalidOperationException($"Subscription type with ID {input.SubscriptionTypeId} not found.");
        }

        var basePrice = subscriptionType.Price;

        var clientSubscriptionsCount = await _clientRepository.GetClientSubscriptionsCountAsync(input.ClientId, cancellationToken);

        var discountPercent = clientSubscriptionsCount > 5 ? 5 : 0;
        var finalPrice = discountPercent > 0
            ? Math.Round(basePrice * 0.95m, 2, MidpointRounding.AwayFromZero)
            : basePrice;

        return new PriceCalculationResult(basePrice, finalPrice, discountPercent);
    }
}
