using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Journals.Queries;

public record GetJournalByIdQuery(Guid Id) : IRequest<JournalDto?>;

public class GetJournalByIdQueryHandler : IRequestHandler<GetJournalByIdQuery, JournalDto?>
{
    private readonly IJournalRepository _repository;

    public GetJournalByIdQueryHandler(IJournalRepository repository)
    {
        _repository = repository;
    }

    public async Task<JournalDto?> Handle(GetJournalByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null
            ? null
            : new JournalDto(entity.Id, entity.Title, entity.IssueNumber, entity.PublicationYear, entity.Publisher);
    }
}
