using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class ClientAddressRepository : IClientAddressRepository
{
    private readonly LibraryDbContext _context;

    public ClientAddressRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<ClientAddress>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.ClientAddresses.ToListAsync(cancellationToken);
    }

    public async Task<ClientAddress?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.ClientAddresses.FindAsync([id], cancellationToken);
    }

    public async Task<ClientAddress> AddAsync(ClientAddress entity, CancellationToken cancellationToken)
    {
        var added = await _context.ClientAddresses.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return added.Entity;
    }

    public async Task<bool> UpdateAsync(ClientAddress entity, CancellationToken cancellationToken)
    {
        _context.ClientAddresses.Update(entity);
        var affected = await _context.SaveChangesAsync(cancellationToken);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.ClientAddresses.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _context.ClientAddresses.Remove(entity);
        var affected = await _context.SaveChangesAsync(cancellationToken);
        return affected > 0;
    }
}
