using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface ISubscriptionTypeRepository
{
    Task<IReadOnlyCollection<SubscriptionType>> GetAllAsync(CancellationToken cancellationToken);

    Task<SubscriptionType?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<SubscriptionType> AddAsync(SubscriptionType entity, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(SubscriptionType entity, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
