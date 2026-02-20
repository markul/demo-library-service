using PaymentService.Client;

namespace LibraryService.Infrastructure.Services;

public sealed class PaymentService(IPaymentServiceClient paymentServiceClient) : IPaymentService
{
    public Task<ICollection<PaymentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return paymentServiceClient.GetAllAsync(cancellationToken);
    }

    public Task<PaymentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return paymentServiceClient.GetByIdAsync(id, cancellationToken);
    }

    public Task<PaymentDto> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        return paymentServiceClient.CreateAsync(request, cancellationToken);
    }
}
