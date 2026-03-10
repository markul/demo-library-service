using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record CheckoutSubscriptionCommand(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey) : IRequest<SubscriptionCheckoutResult?>;

public class CheckoutSubscriptionCommandHandler : IRequestHandler<CheckoutSubscriptionCommand, SubscriptionCheckoutResult?>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CheckoutSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IPaymentRepository paymentRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<SubscriptionCheckoutResult?> Handle(
        CheckoutSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        // Validate client exists
        var clientExists = await _subscriptionRepository.ClientExistsAsync(request.ClientId, cancellationToken);
        if (!clientExists)
        {
            return null;
        }

        // Validate subscription type exists
        var typeExists = await _subscriptionRepository.SubscriptionTypeExistsAsync(request.SubscriptionTypeId, cancellationToken);
        if (!typeExists)
        {
            return null;
        }

        // Create subscription
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            Name = $"Subscription-{request.ClientId}-{request.SubscriptionTypeId}",
            SubscriptionTypeId = request.SubscriptionTypeId,
            IsActive = false,
            StartDateUtc = DateTime.UtcNow,
        };

        var createdSubscription = await _subscriptionRepository.AddAsync(subscription, new[] { request.ClientId }, cancellationToken);
        if (createdSubscription is null)
        {
            return null;
        }

        // Get subscription type to get price for payment amount
        var subscriptionType = createdSubscription.SubscriptionType
            ?? throw new InvalidOperationException("Subscription type is expected to be loaded.");

        // Create payment
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UniqueId = request.IdempotencyKey,
            Amount = subscriptionType.Price,
            SubscriptionId = createdSubscription.Id,
            ClientId = request.ClientId,
            ExternalId = null,
            Status = PaymentStatus.Processing,
        };

        var createdPayment = await _paymentRepository.AddAsync(payment, cancellationToken);
        if (createdPayment is null)
        {
            return null;
        }

        // Call external PaymentService (simulated)
        // In real implementation, this would call an external payment service
        var paymentStatus = await CallPaymentServiceAsync(request.IdempotencyKey, subscriptionType.Price, cancellationToken);

        // Update subscription and payment based on payment result
        if (paymentStatus == PaymentStatus.Paid)
        {
            createdSubscription.IsActive = true;
            createdPayment.Status = PaymentStatus.Paid;
            createdPayment.ExternalId = $"ext-{Guid.NewGuid()}"; // Simulated external payment ID

            await _subscriptionRepository.UpdateAsync(createdSubscription, cancellationToken);
            await _paymentRepository.UpdateAsync(createdPayment, cancellationToken);
        }
        else if (paymentStatus == PaymentStatus.Failed)
        {
            createdPayment.Status = PaymentStatus.Failed;
            await _paymentRepository.UpdateAsync(createdPayment, cancellationToken);
        }
        // If Processing, keep as is (subscription remains inactive)

        return new SubscriptionCheckoutResult(
            createdSubscription.Id,
            paymentStatus);
    }

    private async Task<PaymentStatus> CallPaymentServiceAsync(string idempotencyKey, decimal amount, CancellationToken cancellationToken)
    {
        // Simulated external payment service call
        // In real implementation, this would:
        // 1. Call external payment service API
        // 2. Handle idempotency via the uniqueId
        // 3. Return the payment status from the service
        
        // For now, simulate a successful payment
        // TODO: Integrate with actual PaymentService
        await Task.Delay(100, cancellationToken); // Simulate network delay
        return PaymentStatus.Paid;
    }
}
