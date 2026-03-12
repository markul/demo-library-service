using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Abstractions.Services;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record CheckoutSubscriptionCommand(
    Guid SubscriptionTypeId,
    Guid ClientId) : IRequest<SubscriptionCheckoutResult?>;

public class CheckoutSubscriptionCommandHandler : IRequestHandler<CheckoutSubscriptionCommand, SubscriptionCheckoutResult?>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ISubscriptionPaymentService _paymentService;

    public CheckoutSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IPaymentRepository paymentRepository,
        ISubscriptionPaymentService paymentService)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _paymentService = paymentService;
    }

    public async Task<SubscriptionCheckoutResult?> Handle(CheckoutSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate client and subscription type
        var clientExists = await _subscriptionRepository.ClientExistsAsync(request.ClientId, cancellationToken);
        var subscriptionTypeExists = await _subscriptionRepository.SubscriptionTypeExistsAsync(request.SubscriptionTypeId, cancellationToken);
        
        if (!clientExists || !subscriptionTypeExists)
        {
            return null;
        }

        // Step 2: Calculate subscription price (from subscription type)
        var subscriptionType = await _subscriptionRepository.GetSubscriptionTypeByIdAsync(request.SubscriptionTypeId, cancellationToken);
        if (subscriptionType is null)
        {
            return null;
        }

        var amount = subscriptionType.Price;

        // Step 3: Create subscription
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            Name = subscriptionType.Name,
            SubscriptionTypeId = subscriptionType.Id,
            IsActive = false,
            StartDateUtc = DateTime.UtcNow
        };

        var createdSubscription = await _subscriptionRepository.AddAsync(subscription, new[] { request.ClientId }, cancellationToken);
        if (createdSubscription is null)
        {
            return null;
        }

        // Step 4: Create payment record
        var uniqueId = Guid.NewGuid().ToString();
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UniqueId = uniqueId,
            Amount = amount,
            SubscriptionId = createdSubscription.Id,
            ClientId = request.ClientId,
            Status = PaymentStatus.Processing
        };

        var createdPayment = await _paymentRepository.AddAsync(payment, cancellationToken);
        if (createdPayment is null)
        {
            return null;
        }

        // Step 5: Send payment to PaymentService
        var paymentResult = await _paymentService.ProcessPaymentAsync(
            request.ClientId,
            createdSubscription.Id,
            createdPayment.Id,
            amount,
            uniqueId,
            cancellationToken);

        // Step 6: Update subscription and payment based on payment result
        if (paymentResult.IsSuccess)
        {
            createdSubscription.IsActive = true;
            createdPayment.Status = PaymentStatus.Paid;
            createdPayment.ExternalId = paymentResult.ExternalId;
        }
        else
        {
            createdPayment.Status = PaymentStatus.Failed;
        }

        await _subscriptionRepository.UpdateAsync(createdSubscription, new[] { request.ClientId }, cancellationToken);
        await _paymentRepository.UpdateAsync(createdPayment, cancellationToken);

        // Step 7: Return result
        return new SubscriptionCheckoutResult(
            createdSubscription.Id,
            createdPayment.Id,
            createdPayment.Status.ToString());
    }
}
