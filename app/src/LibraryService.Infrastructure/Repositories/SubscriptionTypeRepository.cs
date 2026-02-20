using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class SubscriptionTypeRepository : ISubscriptionTypeRepository
{
    private readonly LibraryDbContext _dbContext;

    public SubscriptionTypeRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<SubscriptionType>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SubscriptionTypes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Period)
            .ToListAsync(cancellationToken);
    }

    public Task<SubscriptionType?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.SubscriptionTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SubscriptionType> AddAsync(SubscriptionType entity, CancellationToken cancellationToken)
    {
        _dbContext.SubscriptionTypes.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(SubscriptionType entity, CancellationToken cancellationToken)
    {
        _dbContext.SubscriptionTypes.Update(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.SubscriptionTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.SubscriptionTypes.Remove(entity);

        try
        {
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
