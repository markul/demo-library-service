using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class ClientAddressRepository : IClientAddressRepository
{
    private readonly LibraryDbContext _dbContext;

    public ClientAddressRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken)
    {
        return _dbContext.Clients.AnyAsync(x => x.Id == clientId, cancellationToken);
    }

    public async Task<ClientAddress> AddAsync(ClientAddress entity, CancellationToken cancellationToken)
    {
        _dbContext.ClientAddresses.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
