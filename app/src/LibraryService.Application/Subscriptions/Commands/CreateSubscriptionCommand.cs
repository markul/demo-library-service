using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record CreateSubscriptionCommand(
    string Name,
    Guid SubscriptionTypeId,
    bool IsActive,
    DateTime StartDateUtc,
    IReadOnlyCollection<Guid> ClientIds) : IRequest<SubscriptionDto?>;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto?>
{
    private readonly ISubscriptionRepository _repository;

    public CreateSubscriptionCommandHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscriptionDto?> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var clientIds = request.ClientIds.Distinct().ToList();
        var allClientsExist = await _repository.AllClientsExistAsync(clientIds, cancellationToken);
        var typeExists = await _repository.SubscriptionTypeExistsAsync(request.SubscriptionTypeId, cancellationToken);
        if (!allClientsExist || !typeExists)
        {
            return null;
        }

        var entity = new Subscription
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            SubscriptionTypeId = request.SubscriptionTypeId,
            IsActive = request.IsActive,
            StartDateUtc = request.StartDateUtc,
        };

        var created = await _repository.AddAsync(entity, clientIds, cancellationToken);
        var type = created.SubscriptionType
            ?? throw new InvalidOperationException("Subscription type is expected to be loaded.");

        return new SubscriptionDto(
            created.Id,
            created.Name,
            created.SubscriptionTypeId,
            created.IsActive,
            created.StartDateUtc,
            new SubscriptionTypeDto(type.Id, type.Name, type.Period, type.Price),
            created.ClientSubscriptions.Select(x => x.ClientId).ToList());
    }
}
