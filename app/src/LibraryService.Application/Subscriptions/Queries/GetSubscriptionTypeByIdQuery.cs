using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Queries;

public record GetSubscriptionTypeByIdQuery(Guid Id) : IRequest<SubscriptionTypeDto?>;

public class GetSubscriptionTypeByIdQueryHandler : IRequestHandler<GetSubscriptionTypeByIdQuery, SubscriptionTypeDto?>
{
    private readonly ISubscriptionTypeRepository _repository;

    public GetSubscriptionTypeByIdQueryHandler(ISubscriptionTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscriptionTypeDto?> Handle(GetSubscriptionTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null
            ? null
            : new SubscriptionTypeDto(entity.Id, entity.Name, entity.Period, entity.Price);
    }
}
