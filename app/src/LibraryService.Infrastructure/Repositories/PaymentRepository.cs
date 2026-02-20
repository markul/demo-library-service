using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly LibraryDbContext _dbContext;

    public PaymentRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Payment>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Payments
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Payment> AddAsync(Payment entity, CancellationToken cancellationToken)
    {
        _dbContext.Payments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(Payment entity, CancellationToken cancellationToken)
    {
        _dbContext.Payments.Update(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Payments.Remove(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken)
    {
        return _dbContext.Clients.AnyAsync(x => x.Id == clientId, cancellationToken);
    }

    public Task<bool> SubscriptionExistsAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        return _dbContext.Subscriptions.AnyAsync(x => x.Id == subscriptionId, cancellationToken);
    }

    public Task<bool> UniqueIdExistsAsync(string uniqueId, Guid? excludedPaymentId, CancellationToken cancellationToken)
    {
        return _dbContext.Payments.AnyAsync(
            x => x.UniqueId == uniqueId && (!excludedPaymentId.HasValue || x.Id != excludedPaymentId.Value),
            cancellationToken);
    }
}
