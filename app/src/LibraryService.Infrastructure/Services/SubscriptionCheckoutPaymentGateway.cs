using LibraryService.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using PaymentService.Client;

namespace LibraryService.Infrastructure.Services;

public sealed class SubscriptionCheckoutPaymentGateway : ISubscriptionCheckoutPaymentGateway
{
    private readonly IPaymentService _paymentService;
    private readonly string _currency;

    public SubscriptionCheckoutPaymentGateway(IPaymentService paymentService, IConfiguration configuration)
    {
        _paymentService = paymentService;
        _currency = configuration["PaymentService:Currency"] ?? "USD";
    }

    public async Task<SubscriptionCheckoutPaymentResult> CreatePaymentAsync(
        SubscriptionCheckoutPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var createRequest = new CreatePaymentRequest
        {
            ClientId = request.ClientId.ToString("D"),
            Amount = request.Amount,
            Currency = _currency,
            Description = $"Checkout subscription {request.SubscriptionId} type {request.SubscriptionTypeId}",
        };

        try
        {
            var payment = await _paymentService.CreateAsync(createRequest, cancellationToken);
            var externalId = payment.Id.ToString("D");

            return payment.Status switch
            {
                PaymentStatus.Accepted => new SubscriptionCheckoutPaymentResult(
                    SubscriptionCheckoutPaymentStatus.Accepted,
                    externalId),
                PaymentStatus.Rejected => new SubscriptionCheckoutPaymentResult(
                    SubscriptionCheckoutPaymentStatus.Rejected,
                    externalId),
                _ => new SubscriptionCheckoutPaymentResult(SubscriptionCheckoutPaymentStatus.TechnicalFailure, externalId),
            };
        }
        catch (Exception)
        {
            return new SubscriptionCheckoutPaymentResult(SubscriptionCheckoutPaymentStatus.TechnicalFailure, null);
        }
    }
}
