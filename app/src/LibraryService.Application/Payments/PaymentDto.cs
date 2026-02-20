using LibraryService.Domain.Entities;

namespace LibraryService.Application.Payments;

public record PaymentDto(
    Guid Id,
    string UniqueId,
    decimal Amount,
    Guid SubscriptionId,
    Guid ClientId,
    string? ExternalId,
    PaymentStatus Status);

public record CreatePaymentRequest(
    string UniqueId,
    decimal Amount,
    Guid SubscriptionId,
    Guid ClientId,
    string? ExternalId,
    PaymentStatus Status);

public record UpdatePaymentRequest(
    string UniqueId,
    decimal Amount,
    Guid SubscriptionId,
    Guid ClientId,
    string? ExternalId,
    PaymentStatus Status);
