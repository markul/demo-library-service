using LibraryService.Application.Abstractions.Services;
using LibraryService.Application.Ebooks;
using LibraryService.Infrastructure.ConnectedServices.EbookOData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OData.Client;
using OData.QueryBuilder.Builders;

namespace LibraryService.Infrastructure.Services;

public class EbookCatalogService : IEbookCatalogService
{
    private readonly EbookContainer container;
    private readonly ILogger<EbookCatalogService> logger;
    private readonly Uri queryRoot;
    private readonly ODataQueryBuilder<EbookContainer> queryBuilder;

    public EbookCatalogService(EbookContainer container, ILogger<EbookCatalogService> logger)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(logger);
        if (container.BaseUri is null)
        {
            throw new InvalidOperationException("Ebook OData container BaseUri must be configured.");
        }

        this.container = container;
        this.logger = logger;
        queryRoot = container.BaseUri;
        queryBuilder = new ODataQueryBuilder<EbookContainer>(queryRoot);
    }

    protected EbookCatalogService(Uri queryRoot, ILogger<EbookCatalogService>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(queryRoot);

        container = null!;
        this.logger = logger ?? NullLogger<EbookCatalogService>.Instance;
        this.queryRoot = queryRoot;
        queryBuilder = new ODataQueryBuilder<EbookContainer>(queryRoot);
    }

    public async Task<IReadOnlyCollection<EbookCatalogItemDto>> GetBooksAsync(CancellationToken cancellationToken = default)
    {
        var query = queryBuilder
            .For<Book>(_ => _.Books)
            .ByList()
            .ToUri();
        var books = await ExecuteQueryAsync(query, cancellationToken);

        return books.Select(Map).ToArray();
    }

    public async Task<IReadOnlyCollection<EbookCatalogItemDto>> FindBooksByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedName = NormalizeNameForFilter(name);
        if (normalizedName.Length == 0)
        {
            return Array.Empty<EbookCatalogItemDto>();
        }
        var normalizedNameLower = normalizedName.ToLowerInvariant();

        var query = queryBuilder
            .For<Book>(_ => _.Books)
            .ByList()
            .Filter((book, functions) => functions.Contains(functions.ToLower(book.Title), normalizedNameLower))
            .Select(book => new{book.Id, book.Author, book.Genre, book.Language, book.Price, book.Title, book.PublishYear})
            .ToUri();
        var books = await ExecuteQueryAsync(query, cancellationToken);

        return books.Select(Map).ToArray();
    }

    protected virtual async Task<IEnumerable<Book>> ExecuteQueryAsync(
        Uri query,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = NormalizeQueryUri(query);
        var absoluteQuery = BuildAbsoluteQueryUri(normalizedQuery);
        logger.LogInformation("Executing ebook OData query: {QueryUrl}", absoluteQuery);

        try
        {
            return await ExecuteNormalizedQueryAsync(normalizedQuery, cancellationToken);
        }
        catch (DataServiceQueryException exception)
        {
            logger.LogWarning(exception, "Ebook OData query failed for query: {QueryUrl}", absoluteQuery);
            throw new HttpRequestException("Ebook OData query failed.", exception);
        }
        catch (DataServiceClientException exception)
        {
            logger.LogWarning(exception, "Ebook OData request failed for query: {QueryUrl}", absoluteQuery);
            throw new HttpRequestException("Ebook OData request failed.", exception);
        }
    }

    protected virtual Task<IEnumerable<Book>> ExecuteNormalizedQueryAsync(
        Uri query,
        CancellationToken cancellationToken)
    {
        return container.ExecuteAsync<Book>(query, cancellationToken);
    }

    private Uri NormalizeQueryUri(Uri query)
    {
        var relativeQuery = query.IsAbsoluteUri
            ? queryRoot.MakeRelativeUri(query)
            : query;
        var normalizedQuery = relativeQuery.ToString();

        if (normalizedQuery.EndsWith("?", StringComparison.Ordinal))
        {
            normalizedQuery = normalizedQuery[..^1];
        }

        return new Uri(normalizedQuery, UriKind.Relative);
    }

    private Uri BuildAbsoluteQueryUri(Uri query)
    {
        return query.IsAbsoluteUri
            ? query
            : new Uri(queryRoot, query);
    }

    private static EbookCatalogItemDto Map(Book book)
    {
        return new EbookCatalogItemDto(
            book.Id,
            book.Title,
            book.Author,
            book.Genre,
            book.Price,
            book.PublishYear,
            book.Language);
    }

    private static string NormalizeNameForFilter(string name)
    {
        var trimmed = name.Trim();
        var apostropheIndex = trimmed.IndexOfAny(['\'', '\u2019']);
        return apostropheIndex >= 0
            ? trimmed[..apostropheIndex]
            : trimmed;
    }
}
