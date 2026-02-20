namespace EbookService.Models;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int PublishYear { get; set; }

    public string Language { get; set; } = string.Empty;
}
