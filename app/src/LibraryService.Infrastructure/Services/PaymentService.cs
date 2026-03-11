using PaymentService.Client;

namespace LibraryService.Infrastructure.Services;

/// <summary>
/// Implementation of payment service that wraps the external PaymentServiceClient.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentServiceClient _client;

    public PaymentService(IPaymentServiceClient client)
    {
        _client = client;
    }

    public async Task<ICollection<PaymentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetAllAsync(cancellationToken);
    }

    public async Task<PaymentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _client.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        return await _client.CreateAsync(request, cancellationToken);
    }
}
