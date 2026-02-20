using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Subscriptions.Commands;

public record DeleteSubscriptionCommand(Guid Id) : IRequest<bool>;

public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _repository;

    public DeleteSubscriptionCommandHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteSubscriptionCommand request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
