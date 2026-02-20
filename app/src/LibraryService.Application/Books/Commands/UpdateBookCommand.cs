using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Books.Commands;

public record UpdateBookCommand(Guid Id, string Title, string Author, int PublishedYear, string Isbn) : IRequest<bool>;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
{
    private readonly IBookRepository _repository;

    public UpdateBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Title = request.Title;
        existing.Author = request.Author;
        existing.PublishedYear = request.PublishedYear;
        existing.Isbn = request.Isbn;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }
}
