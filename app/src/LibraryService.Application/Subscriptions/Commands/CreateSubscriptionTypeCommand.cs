using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record CreateSubscriptionTypeCommand(string Name, int Period, decimal Price) : IRequest<SubscriptionTypeDto>;

public class CreateSubscriptionTypeCommandHandler : IRequestHandler<CreateSubscriptionTypeCommand, SubscriptionTypeDto>
{
    private readonly ISubscriptionTypeRepository _repository;

    public CreateSubscriptionTypeCommandHandler(ISubscriptionTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscriptionTypeDto> Handle(CreateSubscriptionTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new SubscriptionType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Period = request.Period,
            Price = request.Price,
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return new SubscriptionTypeDto(created.Id, created.Name, created.Period, created.Price);
    }
}
