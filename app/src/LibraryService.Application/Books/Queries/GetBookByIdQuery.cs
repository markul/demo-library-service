using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Books.Queries;

public record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _repository;

    public GetBookByIdQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null
            ? null
            : new BookDto(entity.Id, entity.Title, entity.Author, entity.PublishedYear, entity.Isbn);
    }
}
