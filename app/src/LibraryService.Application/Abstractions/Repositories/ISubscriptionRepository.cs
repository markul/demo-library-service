using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface ISubscriptionRepository
{
    Task<IReadOnlyCollection<Subscription>> GetAllAsync(CancellationToken cancellationToken);

    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Subscription> AddAsync(Subscription entity, IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Subscription entity, IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> AllClientsExistAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);

    Task<bool> SubscriptionTypeExistsAsync(Guid subscriptionTypeId, CancellationToken cancellationToken);
}
