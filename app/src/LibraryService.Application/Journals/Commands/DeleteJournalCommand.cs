using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Journals.Commands;

public record DeleteJournalCommand(Guid Id) : IRequest<bool>;

public class DeleteJournalCommandHandler : IRequestHandler<DeleteJournalCommand, bool>
{
    private readonly IJournalRepository _repository;

    public DeleteJournalCommandHandler(IJournalRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteJournalCommand request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
