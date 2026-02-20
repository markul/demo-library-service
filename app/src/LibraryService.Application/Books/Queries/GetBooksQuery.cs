using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Books.Queries;

public record GetBooksQuery : IRequest<IReadOnlyCollection<BookDto>>;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, IReadOnlyCollection<BookDto>>
{
    private readonly IBookRepository _repository;

    public GetBooksQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(Map).ToList();
    }

    private static BookDto Map(Domain.Entities.Book entity)
    {
        return new BookDto(entity.Id, entity.Title, entity.Author, entity.PublishedYear, entity.Isbn);
    }
}
