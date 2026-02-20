using FluentAssertions;
using LibraryService.Infrastructure.ConnectedServices.EbookOData;
using LibraryService.Infrastructure.Services;

namespace LibraryService.Tests.Unit.Infrastructure;

public class EbookCatalogServiceTests
{
    [Fact]
    public async Task GetBooksAsync_ShouldReturnMappedBooks_WhenODataBooksAreReturned()
    {
        Uri? capturedRequestUri = null;
        var service = CreateService((requestUri, _) =>
        {
            capturedRequestUri = requestUri;
            IEnumerable<Book> books =
            [
                new Book
                {
                    Id = 1,
                    Title = "Dune",
                    Author = "Frank Herbert",
                    Genre = "Science Fiction",
                    Price = 15.5m,
                    PublishYear = 1965,
                    Language = "English"
                }
            ];
            return Task.FromResult(books);
        });

        var result = await service.GetBooksAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.Single().Title.Should().Be("Dune");
        capturedRequestUri.Should().NotBeNull();
        capturedRequestUri!.ToString().Should().Be("Books");
    }

    [Fact]
    public async Task GetBooksAsync_ShouldCallBooksEntitySet()
    {
        Uri? capturedRequestUri = null;
        var service = CreateService((requestUri, _) =>
        {
            capturedRequestUri = requestUri;
            return Task.FromResult<IEnumerable<Book>>([]);
        });

        var result = await service.GetBooksAsync(CancellationToken.None);

        result.Should().BeEmpty();
        capturedRequestUri.Should().NotBeNull();
        capturedRequestUri!.ToString().Should().Be("Books");
    }

    [Fact]
    public async Task FindBooksByNameAsync_ShouldApplyTitleFilter_WhenNameIsProvided()
    {
        Uri? capturedRequestUri = null;
        var service = CreateService((requestUri, _) =>
        {
            capturedRequestUri = requestUri;
            return Task.FromResult<IEnumerable<Book>>([]);
        });

        await service.FindBooksByNameAsync("Dune", CancellationToken.None);

        capturedRequestUri.Should().NotBeNull();
        var decodedQuery = Uri.UnescapeDataString(capturedRequestUri!.ToString());

        decodedQuery.Should().Contain("Books?$filter=");
        decodedQuery.Should().Contain("contains(tolower(Title),'dune')");
    }

    [Fact]
    public async Task FindBooksByNameAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        var service = CreateService((_, _) => Task.FromResult<IEnumerable<Book>>([]));

        var action = async () => await service.FindBooksByNameAsync("   ", CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task FindBooksByNameAsync_ShouldRemoveApostropheAndTrailingSubstring_WhenNameContainsApostrophe()
    {
        Uri? capturedRequestUri = null;
        var service = CreateService((requestUri, _) =>
        {
            capturedRequestUri = requestUri;
            return Task.FromResult<IEnumerable<Book>>([]);
        });

        await service.FindBooksByNameAsync("Sorcerer's", CancellationToken.None);

        capturedRequestUri.Should().NotBeNull();
        var decodedQuery = Uri.UnescapeDataString(capturedRequestUri!.ToString());

        decodedQuery.Should().Contain("contains(tolower(Title),'sorcerer')");
        decodedQuery.Should().NotContain("Sorcerer's");
    }

    private static EbookCatalogService CreateService(
        Func<Uri, CancellationToken, Task<IEnumerable<Book>>> queryExecutor)
    {
        return new TestableEbookCatalogService(queryExecutor);
    }

    private sealed class TestableEbookCatalogService(
        Func<Uri, CancellationToken, Task<IEnumerable<Book>>> queryExecutor)
        : EbookCatalogService(new Uri("http://localhost/odata/", UriKind.Absolute))
    {
        protected override Task<IEnumerable<Book>> ExecuteNormalizedQueryAsync(
            Uri query,
            CancellationToken cancellationToken)
        {
            return queryExecutor(query, cancellationToken);
        }
    }
}

