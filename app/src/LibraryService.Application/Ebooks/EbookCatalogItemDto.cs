namespace LibraryService.Application.Ebooks;

public sealed record EbookCatalogItemDto(
    int Id,
    string Title,
    string Author,
    string Genre,
    decimal Price,
    int PublishYear,
    string Language);
