namespace LibraryService.Domain.Entities;

public class Journal
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int IssueNumber { get; set; }

    public int PublicationYear { get; set; }

    public string Publisher { get; set; } = string.Empty;
}
