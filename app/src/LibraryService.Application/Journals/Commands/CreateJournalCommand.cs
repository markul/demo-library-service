using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Journals.Commands;

public record CreateJournalCommand(string Title, int IssueNumber, int PublicationYear, string Publisher) : IRequest<JournalDto>;

public class CreateJournalCommandHandler : IRequestHandler<CreateJournalCommand, JournalDto>
{
    private readonly IJournalRepository _repository;

    public CreateJournalCommandHandler(IJournalRepository repository)
    {
        _repository = repository;
    }

    public async Task<JournalDto> Handle(CreateJournalCommand request, CancellationToken cancellationToken)
    {
        var entity = new Journal
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            IssueNumber = request.IssueNumber,
            PublicationYear = request.PublicationYear,
            Publisher = request.Publisher,
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return new JournalDto(created.Id, created.Title, created.IssueNumber, created.PublicationYear, created.Publisher);
    }
}
