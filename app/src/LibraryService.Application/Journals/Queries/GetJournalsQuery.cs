using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Journals.Queries;

public record GetJournalsQuery : IRequest<IReadOnlyCollection<JournalDto>>;

public class GetJournalsQueryHandler : IRequestHandler<GetJournalsQuery, IReadOnlyCollection<JournalDto>>
{
    private readonly IJournalRepository _repository;

    public GetJournalsQueryHandler(IJournalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<JournalDto>> Handle(GetJournalsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(Map).ToList();
    }

    private static JournalDto Map(Domain.Entities.Journal entity)
    {
        return new JournalDto(entity.Id, entity.Title, entity.IssueNumber, entity.PublicationYear, entity.Publisher);
    }
}
