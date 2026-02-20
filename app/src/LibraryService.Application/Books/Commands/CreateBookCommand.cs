using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Books.Commands;

public record CreateBookCommand(string Title, string Author, int PublishedYear, string Isbn) : IRequest<BookDto>;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IBookRepository _repository;

    public CreateBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var entity = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            PublishedYear = request.PublishedYear,
            Isbn = request.Isbn,
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return new BookDto(created.Id, created.Title, created.Author, created.PublishedYear, created.Isbn);
    }
}
