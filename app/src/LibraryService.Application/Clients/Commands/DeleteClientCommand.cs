using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Clients.Commands;

public record DeleteClientCommand(Guid Id) : IRequest<bool>;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
{
    private readonly IClientRepository _repository;

    public DeleteClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
