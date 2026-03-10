using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record SubscriptionCheckoutCommand(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey) : IRequest<SubscriptionCheckoutResult?>;

public class SubscriptionCheckoutCommandHandler : IRequestHandler<SubscriptionCheckoutCommand, SubscriptionCheckoutResult?>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionTypeRepository _subscriptionTypeRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IClientRepository _clientRepository;

    public SubscriptionCheckoutCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionTypeRepository subscriptionTypeRepository,
        IPaymentRepository paymentRepository,
        IClientRepository clientRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionTypeRepository = subscriptionTypeRepository;
        _paymentRepository = paymentRepository;
        _clientRepository = clientRepository;
    }

    public async Task<SubscriptionCheckoutResult?> Handle(SubscriptionCheckoutCommand request, CancellationToken cancellationToken)
    {
        // 1. Idempotency check - if payment with this unique id exists, return existing result
        var existingPayment = await _paymentRepository.GetByUniqueIdAsync(request.IdempotencyKey, cancellationToken);
        if (existingPayment is not null)
        {
            return new SubscriptionCheckoutResult(
                existingPayment.SubscriptionId,
                existingPayment.Status.ToString());
        }

        // 2. Validate client exists
        var clientExists = await _clientRepository.ExistsAsync(request.ClientId, cancellationToken);
        if (!clientExists)
        {
            return null;
        }

        // 3. Validate subscription type exists and get price
        var subscriptionType = await _subscriptionTypeRepository.GetByIdAsync(request.SubscriptionTypeId, cancellationToken);
        if (subscriptionType is null)
        {
            return null;
        }

        // 4. Create subscription (inactive)
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            Name = $"Subscription for client {request.ClientId}",
            SubscriptionTypeId = request.SubscriptionTypeId,
            IsActive = false,
            StartDateUtc = DateTime.UtcNow,
        };

        // 5. Create payment (processing status)
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UniqueId = request.IdempotencyKey,
            Amount = subscriptionType.Price,
            SubscriptionId = subscription.Id,
            ClientId = request.ClientId,
            Status = PaymentStatus.Processing,
        };

        // Save subscription and payment
        await _subscriptionRepository.AddAsync(subscription, new List<Guid> { request.ClientId }, cancellationToken);
        await _paymentRepository.AddAsync(payment, cancellationToken);

        // Note: Payment processing is handled by a separate process/service
        // The payment will be processed asynchronously and the status will be updated via webhook or polling

        return new SubscriptionCheckoutResult(subscription.Id, payment.Status.ToString());
    }
}
