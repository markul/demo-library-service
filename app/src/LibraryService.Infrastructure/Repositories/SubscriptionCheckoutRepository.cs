using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace LibraryService.Infrastructure.Repositories;

public class SubscriptionCheckoutRepository : ISubscriptionCheckoutRepository
{
    private readonly LibraryDbContext _dbContext;

    public SubscriptionCheckoutRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SubscriptionCheckoutExistingPayment?> GetByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        return (
            from payment in _dbContext.Payments.AsNoTracking()
            join subscription in _dbContext.Subscriptions.AsNoTracking()
                on payment.SubscriptionId equals subscription.Id
            where payment.UniqueId == idempotencyKey
            select new SubscriptionCheckoutExistingPayment(
                payment.ClientId,
                subscription.SubscriptionTypeId,
                payment.SubscriptionId,
                payment.Status))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<SubscriptionCheckoutInputData?> GetCheckoutInputDataAsync(
        Guid clientId,
        Guid subscriptionTypeId,
        CancellationToken cancellationToken)
    {
        var clientExists = await _dbContext.Clients
            .AsNoTracking()
            .AnyAsync(x => x.Id == clientId, cancellationToken);
        if (!clientExists)
        {
            return null;
        }

        var subscriptionType = await _dbContext.SubscriptionTypes
            .AsNoTracking()
            .Where(x => x.Id == subscriptionTypeId)
            .Select(x => new { x.Price, x.Name })
            .SingleOrDefaultAsync(cancellationToken);
        if (subscriptionType is null)
        {
            return null;
        }

        var clientSubscriptionsCount = await _dbContext.ClientSubscriptions
            .AsNoTracking()
            .CountAsync(x => x.ClientId == clientId, cancellationToken);

        return new SubscriptionCheckoutInputData(
            subscriptionType.Price,
            subscriptionType.Name,
            clientSubscriptionsCount);
    }

    public async Task<SubscriptionCheckoutPendingResult?> CreatePendingCheckoutAsync(
        SubscriptionCheckoutPendingCreateRequest request,
        CancellationToken cancellationToken)
    {
        var idempotencyExists = await _dbContext.Payments
            .AnyAsync(x => x.UniqueId == request.IdempotencyKey, cancellationToken);
        if (idempotencyExists)
        {
            return null;
        }

        IDbContextTransaction? transaction = null;
        if (_dbContext.Database.IsRelational())
        {
            transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        try
        {
            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                Name = request.SubscriptionName,
                SubscriptionTypeId = request.SubscriptionTypeId,
                IsActive = false,
                StartDateUtc = request.StartDateUtc,
            };
            _dbContext.Subscriptions.Add(subscription);

            _dbContext.ClientSubscriptions.Add(new ClientSubscription
            {
                ClientId = request.ClientId,
                SubscriptionId = subscription.Id,
            });

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UniqueId = request.IdempotencyKey,
                Amount = request.Amount,
                SubscriptionId = subscription.Id,
                ClientId = request.ClientId,
                ExternalId = null,
                Status = PaymentStatus.Processing,
            };
            _dbContext.Payments.Add(payment);

            await _dbContext.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return new SubscriptionCheckoutPendingResult(subscription.Id, payment.Id);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            return null;
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    public async Task UpdateCheckoutResultAsync(
        Guid subscriptionId,
        Guid paymentId,
        PaymentStatus paymentStatus,
        bool isSubscriptionActive,
        string? externalId,
        CancellationToken cancellationToken)
    {
        var payment = await _dbContext.Payments.FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken)
            ?? throw new InvalidOperationException($"Payment '{paymentId}' was not found.");
        var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == subscriptionId, cancellationToken)
            ?? throw new InvalidOperationException($"Subscription '{subscriptionId}' was not found.");

        payment.Status = paymentStatus;
        payment.ExternalId = externalId;
        subscription.IsActive = isSubscriptionActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException &&
               postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
