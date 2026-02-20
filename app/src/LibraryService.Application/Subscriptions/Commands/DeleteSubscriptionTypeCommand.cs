using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record DeleteSubscriptionTypeCommand(Guid Id) : IRequest<bool>;

public class DeleteSubscriptionTypeCommandHandler : IRequestHandler<DeleteSubscriptionTypeCommand, bool>
{
    private readonly ISubscriptionTypeRepository _repository;

    public DeleteSubscriptionTypeCommandHandler(ISubscriptionTypeRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteSubscriptionTypeCommand request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
