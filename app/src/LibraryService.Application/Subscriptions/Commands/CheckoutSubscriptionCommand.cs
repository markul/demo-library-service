using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Abstractions.Services;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record CheckoutSubscriptionCommand(
    Guid ClientId,
    Guid SubscriptionTypeId,
    string IdempotencyKey) : IRequest<CheckoutSubscriptionCommandResult>;

public sealed record CheckoutSubscriptionCommandResult(
    CheckoutSubscriptionOutcome Outcome,
    CheckoutSubscriptionResponse? Response);

public enum CheckoutSubscriptionOutcome
{
    Created = 1,
    Ok = 2,
    Accepted = 3,
    PaymentRequired = 4,
    NotFound = 5,
    Conflict = 6,
}

public class CheckoutSubscriptionCommandHandler
    : IRequestHandler<CheckoutSubscriptionCommand, CheckoutSubscriptionCommandResult>
{
    private const string CheckoutSubscriptionName = "Checkout subscription";
    private readonly ISubscriptionCheckoutRepository _repository;
    private readonly ISubscriptionCheckoutPaymentGateway _paymentGateway;

    public CheckoutSubscriptionCommandHandler(
        ISubscriptionCheckoutRepository repository,
        ISubscriptionCheckoutPaymentGateway paymentGateway)
    {
        _repository = repository;
        _paymentGateway = paymentGateway;
    }

    public async Task<CheckoutSubscriptionCommandResult> Handle(
        CheckoutSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return MapExistingCheckout(existing, request);
        }

        var checkoutInput = await _repository.GetCheckoutInputDataAsync(
            request.ClientId,
            request.SubscriptionTypeId,
            cancellationToken);
        if (checkoutInput is null)
        {
            return new CheckoutSubscriptionCommandResult(CheckoutSubscriptionOutcome.NotFound, null);
        }

        var finalPrice = CalculateFinalPrice(checkoutInput.BasePrice, checkoutInput.ClientSubscriptionsCount);
        var pending = await _repository.CreatePendingCheckoutAsync(
            new SubscriptionCheckoutPendingCreateRequest(
                request.ClientId,
                request.SubscriptionTypeId,
                request.IdempotencyKey,
                CheckoutSubscriptionName,
                DateTime.UtcNow,
                finalPrice),
            cancellationToken);

        if (pending is null)
        {
            var concurrentExisting = await _repository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
            if (concurrentExisting is null)
            {
                return new CheckoutSubscriptionCommandResult(CheckoutSubscriptionOutcome.Conflict, null);
            }

            return MapExistingCheckout(concurrentExisting, request);
        }

        var paymentResult = await _paymentGateway.CreatePaymentAsync(
            new SubscriptionCheckoutPaymentRequest(
                request.ClientId,
                pending.SubscriptionId,
                request.SubscriptionTypeId,
                finalPrice,
                request.IdempotencyKey),
            cancellationToken);

        var (paymentStatus, isSubscriptionActive, externalId) = paymentResult.Status switch
        {
            SubscriptionCheckoutPaymentStatus.Accepted => (PaymentStatus.Paid, true, paymentResult.ExternalId),
            SubscriptionCheckoutPaymentStatus.Rejected => (PaymentStatus.Failed, false, paymentResult.ExternalId),
            _ => (PaymentStatus.Processing, false, (string?)null),
        };

        await _repository.UpdateCheckoutResultAsync(
            pending.SubscriptionId,
            pending.PaymentId,
            paymentStatus,
            isSubscriptionActive,
            externalId,
            cancellationToken);

        var outcome = paymentStatus switch
        {
            PaymentStatus.Paid => CheckoutSubscriptionOutcome.Created,
            PaymentStatus.Failed => CheckoutSubscriptionOutcome.PaymentRequired,
            _ => CheckoutSubscriptionOutcome.Accepted,
        };

        return new CheckoutSubscriptionCommandResult(
            outcome,
            new CheckoutSubscriptionResponse(pending.SubscriptionId, paymentStatus.ToString()));
    }

    private static CheckoutSubscriptionCommandResult MapExistingCheckout(
        SubscriptionCheckoutExistingPayment existing,
        CheckoutSubscriptionCommand request)
    {
        if (existing.ClientId != request.ClientId || existing.SubscriptionTypeId != request.SubscriptionTypeId)
        {
            return new CheckoutSubscriptionCommandResult(CheckoutSubscriptionOutcome.Conflict, null);
        }

        var outcome = existing.PaymentStatus switch
        {
            PaymentStatus.Paid => CheckoutSubscriptionOutcome.Ok,
            PaymentStatus.Failed => CheckoutSubscriptionOutcome.PaymentRequired,
            _ => CheckoutSubscriptionOutcome.Accepted,
        };

        return new CheckoutSubscriptionCommandResult(
            outcome,
            new CheckoutSubscriptionResponse(existing.SubscriptionId, existing.PaymentStatus.ToString()));
    }

    private static decimal CalculateFinalPrice(decimal basePrice, int clientSubscriptionsCount)
    {
        if (clientSubscriptionsCount <= 5)
        {
            return basePrice;
        }

        return Math.Round(basePrice * 0.95m, 2, MidpointRounding.AwayFromZero);
    }
}
