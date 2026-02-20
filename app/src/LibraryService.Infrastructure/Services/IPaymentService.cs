using PaymentService.Client;

namespace LibraryService.Infrastructure.Services;

public interface IPaymentService
{
    Task<ICollection<PaymentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PaymentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaymentDto> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
}
