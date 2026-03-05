using LibraryService.Application.Ebooks;

namespace LibraryService.Application.Abstractions.Services;

public interface IEbookCatalogService
{
    Task<IReadOnlyCollection<EbookCatalogItemDto>> GetBooksAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EbookCatalogItemDto>> FindBooksAsync(
        string name,
        CancellationToken cancellationToken = default);
}
