using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface IPaymentRepository
{
    Task<IReadOnlyCollection<Payment>> GetAllAsync(CancellationToken cancellationToken);

    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Payment> AddAsync(Payment entity, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Payment entity, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken);

    Task<bool> SubscriptionExistsAsync(Guid subscriptionId, CancellationToken cancellationToken);

    Task<bool> UniqueIdExistsAsync(string uniqueId, Guid? excludedPaymentId, CancellationToken cancellationToken);
}
