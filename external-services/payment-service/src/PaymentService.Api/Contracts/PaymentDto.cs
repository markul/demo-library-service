using PaymentService.Infrastructure.Models;

namespace PaymentService.Api.Contracts;

public sealed record PaymentDto(
    Guid Id,
    string ClientId,
    decimal Amount,
    string Currency,
    string? Description,
    PaymentStatus Status,
    DateTime CreatedAtUtc);
