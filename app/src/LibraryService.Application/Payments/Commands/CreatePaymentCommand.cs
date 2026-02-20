using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Payments.Commands;

public record CreatePaymentCommand(
    string UniqueId,
    decimal Amount,
    Guid SubscriptionId,
    Guid ClientId,
    string? ExternalId,
    PaymentStatus Status) : IRequest<PaymentDto?>;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto?>
{
    private readonly IPaymentRepository _repository;

    public CreatePaymentCommandHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentDto?> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _repository.ClientExistsAsync(request.ClientId, cancellationToken);
        var subscriptionExists = await _repository.SubscriptionExistsAsync(request.SubscriptionId, cancellationToken);
        var uniqueIdExists = await _repository.UniqueIdExistsAsync(request.UniqueId, null, cancellationToken);
        if (!clientExists || !subscriptionExists || uniqueIdExists)
        {
            return null;
        }

        var entity = new Payment
        {
            Id = Guid.NewGuid(),
            UniqueId = request.UniqueId,
            Amount = request.Amount,
            SubscriptionId = request.SubscriptionId,
            ClientId = request.ClientId,
            ExternalId = request.ExternalId,
            Status = request.Status,
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return new PaymentDto(
            created.Id,
            created.UniqueId,
            created.Amount,
            created.SubscriptionId,
            created.ClientId,
            created.ExternalId,
            created.Status);
    }
}
