namespace LibraryService.Application.Journals;

public record JournalDto(Guid Id, string Title, int IssueNumber, int PublicationYear, string Publisher);

public record CreateJournalRequest(string Title, int IssueNumber, int PublicationYear, string Publisher);

public record UpdateJournalRequest(string Title, int IssueNumber, int PublicationYear, string Publisher);
