using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Queries;

public record GetSubscriptionByIdQuery(Guid Id) : IRequest<SubscriptionDto?>;

public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, SubscriptionDto?>
{
    private readonly ISubscriptionRepository _repository;

    public GetSubscriptionByIdQueryHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscriptionDto?> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

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
