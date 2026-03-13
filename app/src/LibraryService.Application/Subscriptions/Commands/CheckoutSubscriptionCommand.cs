using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record CheckoutSubscriptionCommand(
    Guid ClientId,
    Guid SubscriptionTypeId,
    string IdempotencyKey) : IRequest<SubscriptionCheckoutResult>;
