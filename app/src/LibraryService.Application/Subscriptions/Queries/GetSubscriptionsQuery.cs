using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Queries;

public record GetSubscriptionsQuery : IRequest<IReadOnlyCollection<SubscriptionDto>>;

public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, IReadOnlyCollection<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _repository;

    public GetSubscriptionsQueryHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SubscriptionDto>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(Map).ToList();
    }

    private static SubscriptionDto Map(Domain.Entities.Subscription entity)
    {
        var type = entity.SubscriptionType
            ?? throw new InvalidOperationException("Subscription type is expected to be loaded.");

        return new SubscriptionDto(
            entity.Id,
            entity.Name,
            entity.SubscriptionTypeId,
            entity.IsActive,
            entity.StartDateUtc,
            new SubscriptionTypeDto(type.Id, type.Name, type.Period, type.Price),
            entity.ClientSubscriptions.Select(x => x.ClientId).ToList());
    }
}
