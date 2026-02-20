using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Payments.Queries;

public record GetPaymentsQuery : IRequest<IReadOnlyCollection<PaymentDto>>;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, IReadOnlyCollection<PaymentDto>>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentsQueryHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities
            .Select(x => new PaymentDto(x.Id, x.UniqueId, x.Amount, x.SubscriptionId, x.ClientId, x.ExternalId, x.Status))
            .ToList();
    }
}
