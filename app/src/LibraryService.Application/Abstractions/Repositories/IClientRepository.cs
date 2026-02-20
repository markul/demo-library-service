using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface IClientRepository
{
    Task<IReadOnlyCollection<Client>> GetAllAsync(CancellationToken cancellationToken);

    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Client> AddAsync(Client entity, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Client entity, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
