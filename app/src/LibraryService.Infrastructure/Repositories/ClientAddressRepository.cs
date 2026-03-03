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

    public async Task<IReadOnlyCollection<ClientAddress>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ClientAddresses
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<ClientAddress?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.ClientAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ClientAddress> AddAsync(ClientAddress entity, CancellationToken cancellationToken)
    {
        _dbContext.ClientAddresses.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(ClientAddress entity, CancellationToken cancellationToken)
    {
        _dbContext.ClientAddresses.Update(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ClientAddresses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.ClientAddresses.Remove(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
