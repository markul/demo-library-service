namespace LibraryService.Application.Ebooks;

/// <summary>
/// DTO for ebook search results containing only Id and Title.
/// </summary>
public sealed record EbookSearchResultDto(
    int Id,
    string Title);
