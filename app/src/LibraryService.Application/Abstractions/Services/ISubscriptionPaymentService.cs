namespace LibraryService.Application.Abstractions.Services;

public record PaymentProcessingResult(bool IsSuccess, string? ExternalId);

public interface ISubscriptionPaymentService
{
    Task<PaymentProcessingResult> ProcessPaymentAsync(
        Guid clientId,
        Guid subscriptionId,
        Guid paymentId,
        decimal amount,
        string uniqueId,
        CancellationToken cancellationToken);
}
