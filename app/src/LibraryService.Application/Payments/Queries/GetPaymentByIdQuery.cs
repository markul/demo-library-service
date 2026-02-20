using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Payments.Queries;

public record GetPaymentByIdQuery(Guid Id) : IRequest<PaymentDto?>;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentByIdQueryHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null
            ? null
            : new PaymentDto(entity.Id, entity.UniqueId, entity.Amount, entity.SubscriptionId, entity.ClientId, entity.ExternalId, entity.Status);
    }
}
