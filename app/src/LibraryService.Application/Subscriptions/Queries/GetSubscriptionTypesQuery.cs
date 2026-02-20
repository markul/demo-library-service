using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Queries;

public record GetSubscriptionTypesQuery : IRequest<IReadOnlyCollection<SubscriptionTypeDto>>;

public class GetSubscriptionTypesQueryHandler : IRequestHandler<GetSubscriptionTypesQuery, IReadOnlyCollection<SubscriptionTypeDto>>
{
    private readonly ISubscriptionTypeRepository _repository;

    public GetSubscriptionTypesQueryHandler(ISubscriptionTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SubscriptionTypeDto>> Handle(GetSubscriptionTypesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(x => new SubscriptionTypeDto(x.Id, x.Name, x.Period, x.Price)).ToList();
    }
}
