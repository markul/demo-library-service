using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface IClientAddressRepository
{
    Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken);

    Task<ClientAddress> AddAsync(ClientAddress entity, CancellationToken cancellationToken);
}
