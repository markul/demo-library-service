using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record UpdateSubscriptionCommand(
    Guid Id,
    string Name,
    Guid SubscriptionTypeId,
    bool IsActive,
    DateTime StartDateUtc,
    IReadOnlyCollection<Guid> ClientIds) : IRequest<bool>;

public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _repository;

    public UpdateSubscriptionCommandHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        var clientIds = request.ClientIds.Distinct().ToList();
        var allClientsExist = await _repository.AllClientsExistAsync(clientIds, cancellationToken);
        var typeExists = await _repository.SubscriptionTypeExistsAsync(request.SubscriptionTypeId, cancellationToken);
        if (!allClientsExist || !typeExists)
        {
            return false;
        }

        existing.Name = request.Name;
        existing.SubscriptionTypeId = request.SubscriptionTypeId;
        existing.IsActive = request.IsActive;
        existing.StartDateUtc = request.StartDateUtc;

        return await _repository.UpdateAsync(existing, clientIds, cancellationToken);
    }
}
