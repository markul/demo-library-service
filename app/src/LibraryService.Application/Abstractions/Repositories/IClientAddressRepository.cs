using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface IClientAddressRepository
{
    Task<IReadOnlyCollection<ClientAddress>> GetAllAsync(CancellationToken cancellationToken);

    Task<ClientAddress?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ClientAddress>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);

    Task<ClientAddress> AddAsync(ClientAddress entity, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
