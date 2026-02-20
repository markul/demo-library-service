using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Payments.Commands;

public record UpdatePaymentCommand(
    Guid Id,
    string UniqueId,
    decimal Amount,
    Guid SubscriptionId,
    Guid ClientId,
    string? ExternalId,
    Domain.Entities.PaymentStatus Status) : IRequest<bool>;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, bool>
{
    private readonly IPaymentRepository _repository;

    public UpdatePaymentCommandHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        var clientExists = await _repository.ClientExistsAsync(request.ClientId, cancellationToken);
        var subscriptionExists = await _repository.SubscriptionExistsAsync(request.SubscriptionId, cancellationToken);
        var uniqueIdExists = await _repository.UniqueIdExistsAsync(request.UniqueId, request.Id, cancellationToken);
        if (!clientExists || !subscriptionExists || uniqueIdExists)
        {
            return false;
        }

        existing.UniqueId = request.UniqueId;
        existing.Amount = request.Amount;
        existing.SubscriptionId = request.SubscriptionId;
        existing.ClientId = request.ClientId;
        existing.ExternalId = request.ExternalId;
        existing.Status = request.Status;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }
}
