using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Books.Commands;

public record DeleteBookCommand(Guid Id) : IRequest<bool>;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IBookRepository _repository;

    public DeleteBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
