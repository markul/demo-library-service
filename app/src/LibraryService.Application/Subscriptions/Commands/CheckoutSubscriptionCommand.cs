using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Abstractions.Services;
using LibraryService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryService.Application.Subscriptions.Commands;

/// <summary>
/// Command for processing a subscription checkout.
/// </summary>
/// <param name="SubscriptionTypeId">The subscription type identifier.</param>
/// <param name="ClientId">The client identifier.</param>
/// <param name="IdempotencyKey">Unique key for idempotent payment processing.</param>
public record CheckoutSubscriptionCommand(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey) : IRequest<CheckoutResult?>;

/// <summary>
/// Result of a subscription checkout operation.
/// </summary>
/// <param name="SubscriptionId">The created subscription identifier.</param>
/// <param name="PaymentStatus">The final payment status.</param>
/// <param name="Amount">The payment amount.</param>
public record CheckoutResult(
    Guid SubscriptionId,
    PaymentStatus PaymentStatus,
    decimal Amount);

/// <summary>
/// Handler for the checkout subscription command.
/// </summary>
public class CheckoutSubscriptionCommandHandler : IRequestHandler<CheckoutSubscriptionCommand, CheckoutResult?>
{
    private readonly IClientRepository _clientRepository;
    private readonly ISubscriptionTypeRepository _subscriptionTypeRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ISubscriptionPricingService _pricingService;
    private readonly ISubscriptionPaymentService _paymentService;
    private readonly ILogger<CheckoutSubscriptionCommandHandler> _logger;

    public CheckoutSubscriptionCommandHandler(
        IClientRepository clientRepository,
        ISubscriptionTypeRepository subscriptionTypeRepository,
        ISubscriptionRepository subscriptionRepository,
        IPaymentRepository paymentRepository,
        ISubscriptionPricingService pricingService,
        ISubscriptionPaymentService paymentService,
        ILogger<CheckoutSubscriptionCommandHandler> logger)
    {
        _clientRepository = clientRepository;
        _subscriptionTypeRepository = subscriptionTypeRepository;
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _pricingService = pricingService;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<CheckoutResult?> Handle(CheckoutSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate client and subscription type
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            _logger.LogWarning("Checkout failed: Client {ClientId} not found", request.ClientId);
            return null;
        }

        var subscriptionType = await _subscriptionTypeRepository.GetByIdAsync(request.SubscriptionTypeId, cancellationToken);
        if (subscriptionType is null)
        {
            _logger.LogWarning("Checkout failed: Subscription type {SubscriptionTypeId} not found", request.SubscriptionTypeId);
            return null;
        }

        // Check for duplicate idempotency key
        var existingPayment = await _paymentRepository.GetByUniqueIdAsync(request.IdempotencyKey, cancellationToken);
        if (existingPayment is not null)
        {
            _logger.LogWarning("Checkout failed: Idempotency key {IdempotencyKey} already used", request.IdempotencyKey);
            return new CheckoutResult(existingPayment.SubscriptionId, existingPayment.Status, existingPayment.Amount);
        }

        // Step 2: Calculate price
        var amount = await _pricingService.CalculatePriceAsync(request.SubscriptionTypeId, request.ClientId, cancellationToken);
        _logger.LogInformation("Calculated price: {Amount} for client {ClientId}", amount, request.ClientId);

        // Step 3: Create subscription (inactive)
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            Name = $"{subscriptionType.Name} - {client.FirstName} {client.LastName}",
            SubscriptionTypeId = request.SubscriptionTypeId,
            IsActive = false,
            StartDateUtc = DateTime.UtcNow
        };

        var createdSubscription = await _subscriptionRepository.AddAsync(subscription, new List<Guid> { request.ClientId }, cancellationToken);
        _logger.LogInformation("Created subscription {SubscriptionId} (inactive)", createdSubscription.Id);

        // Step 4: Create payment (processing)
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UniqueId = request.IdempotencyKey,
            Amount = amount,
            SubscriptionId = createdSubscription.Id,
            ClientId = request.ClientId,
            Status = PaymentStatus.Processing
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);
        _logger.LogInformation("Created payment {PaymentId} with status Processing", payment.Id);

        // Step 5: Process payment via payment service
        SubscriptionPaymentResult paymentResult;
        try
        {
            paymentResult = await _paymentService.ProcessPaymentAsync(
                request.ClientId,
                createdSubscription.Id,
                amount,
                request.IdempotencyKey,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payment service error for subscription {SubscriptionId}", createdSubscription.Id);
            // Keep payment in Processing status for retry
            return new CheckoutResult(createdSubscription.Id, PaymentStatus.Processing, amount);
        }

        // Update subscription and payment based on result
        if (paymentResult.IsSuccess)
        {
            // Activate subscription
            createdSubscription.IsActive = true;
            await _subscriptionRepository.UpdateAsync(createdSubscription, new List<Guid> { request.ClientId }, cancellationToken);

            // Update payment status
            payment.Status = PaymentStatus.Paid;
            payment.ExternalId = paymentResult.ExternalId;
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            _logger.LogInformation(
                "Payment successful. Subscription {SubscriptionId} activated. External payment ID: {ExternalId}",
                createdSubscription.Id, paymentResult.ExternalId);
        }
        else
        {
            // Mark payment as failed
            payment.Status = PaymentStatus.Failed;
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            _logger.LogWarning(
                "Payment failed for subscription {SubscriptionId}. Reason: {ErrorMessage}",
                createdSubscription.Id, paymentResult.ErrorMessage);
        }

        // Step 6: Return result
        return new CheckoutResult(createdSubscription.Id, payment.Status, amount);
    }
}
