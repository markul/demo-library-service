using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record UpdateSubscriptionTypeCommand(Guid Id, string Name, int Period, decimal Price) : IRequest<bool>;

public class UpdateSubscriptionTypeCommandHandler : IRequestHandler<UpdateSubscriptionTypeCommand, bool>
{
    private readonly ISubscriptionTypeRepository _repository;

    public UpdateSubscriptionTypeCommandHandler(ISubscriptionTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateSubscriptionTypeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Name = request.Name;
        existing.Period = request.Period;
        existing.Price = request.Price;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }
}
