namespace LibraryService.Domain.Entities;

public class Book
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public int PublishedYear { get; set; }

    public string Isbn { get; set; } = string.Empty;
}
