using LibraryService.Application.Abstractions.Services;
using MediatR;

namespace LibraryService.Application.Ebooks.Queries;

public sealed record GetEbookCatalogQuery : IRequest<IReadOnlyCollection<EbookCatalogItemDto>>;
public sealed record GetEbookCatalogByNameQuery(string Name) : IRequest<IReadOnlyCollection<EbookSearchResultDto>>;

public sealed class GetEbookCatalogQueryHandler(IEbookCatalogService ebookCatalogService)
    : IRequestHandler<GetEbookCatalogQuery, IReadOnlyCollection<EbookCatalogItemDto>>
{
    public Task<IReadOnlyCollection<EbookCatalogItemDto>> Handle(
        GetEbookCatalogQuery request,
        CancellationToken cancellationToken)
    {
        return ebookCatalogService.GetBooksAsync(cancellationToken);
    }
}

public sealed class GetEbookCatalogByNameQueryHandler(IEbookCatalogService ebookCatalogService)
    : IRequestHandler<GetEbookCatalogByNameQuery, IReadOnlyCollection<EbookSearchResultDto>>
{
    public Task<IReadOnlyCollection<EbookSearchResultDto>> Handle(
        GetEbookCatalogByNameQuery request,
        CancellationToken cancellationToken)
    {
        return ebookCatalogService.FindBooksByNameAsync(request.Name, cancellationToken);
    }
}
