using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly LibraryDbContext _dbContext;

    public SubscriptionRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Subscription>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
            .AsNoTracking()
            .Include(x => x.SubscriptionType)
            .Include(x => x.ClientSubscriptions)
            .OrderByDescending(x => x.StartDateUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Subscriptions
            .AsNoTracking()
            .Include(x => x.SubscriptionType)
            .Include(x => x.ClientSubscriptions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Subscription> AddAsync(Subscription entity, IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken)
    {
        _dbContext.Subscriptions.Add(entity);

        foreach (var clientId in clientIds.Distinct())
        {
            _dbContext.ClientSubscriptions.Add(new ClientSubscription
            {
                ClientId = clientId,
                SubscriptionId = entity.Id,
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await LoadByIdAsync(entity.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created subscription could not be loaded.");
    }

    public async Task<bool> UpdateAsync(Subscription entity, IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken)
    {
        _dbContext.Subscriptions.Update(entity);

        var existingLinks = await _dbContext.ClientSubscriptions
            .Where(x => x.SubscriptionId == entity.Id)
            .ToListAsync(cancellationToken);
        _dbContext.ClientSubscriptions.RemoveRange(existingLinks);

        foreach (var clientId in clientIds.Distinct())
        {
            _dbContext.ClientSubscriptions.Add(new ClientSubscription
            {
                ClientId = clientId,
                SubscriptionId = entity.Id,
            });
        }

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        var existingLinks = await _dbContext.ClientSubscriptions
            .Where(x => x.SubscriptionId == id)
            .ToListAsync(cancellationToken);
        _dbContext.ClientSubscriptions.RemoveRange(existingLinks);
        _dbContext.Subscriptions.Remove(entity);

        try
        {
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> AllClientsExistAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken)
    {
        var normalizedIds = clientIds.Distinct().ToList();
        if (normalizedIds.Count == 0)
        {
            return true;
        }

        var existingCount = await _dbContext.Clients
            .CountAsync(x => normalizedIds.Contains(x.Id), cancellationToken);
        return existingCount == normalizedIds.Count;
    }

    public Task<bool> SubscriptionTypeExistsAsync(Guid subscriptionTypeId, CancellationToken cancellationToken)
    {
        return _dbContext.SubscriptionTypes.AnyAsync(x => x.Id == subscriptionTypeId, cancellationToken);
    }

    private Task<Subscription?> LoadByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Subscriptions
            .AsNoTracking()
            .Include(x => x.SubscriptionType)
            .Include(x => x.ClientSubscriptions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
