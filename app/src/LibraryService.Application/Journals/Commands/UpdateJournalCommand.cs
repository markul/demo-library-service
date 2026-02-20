using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Journals.Commands;

public record UpdateJournalCommand(Guid Id, string Title, int IssueNumber, int PublicationYear, string Publisher) : IRequest<bool>;

public class UpdateJournalCommandHandler : IRequestHandler<UpdateJournalCommand, bool>
{
    private readonly IJournalRepository _repository;

    public UpdateJournalCommandHandler(IJournalRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateJournalCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Title = request.Title;
        existing.IssueNumber = request.IssueNumber;
        existing.PublicationYear = request.PublicationYear;
        existing.Publisher = request.Publisher;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }
}
