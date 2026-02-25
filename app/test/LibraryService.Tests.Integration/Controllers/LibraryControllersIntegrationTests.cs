using FluentAssertions;
using LibraryService.Application.Books;
using LibraryService.Application.Clients;
using LibraryService.Application.Ebooks;
using LibraryService.Application.Journals;
using LibraryService.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace LibraryService.Tests.Integration.Controllers;

public class LibraryControllersIntegrationTests : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client;

    public LibraryControllersIntegrationTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
        factory.ResetAndSeed();
    }

    [Fact]
    public async Task GetBooks_ShouldReturnSeededData()
    {
        // Arrange
        const string requestPath = "/api/books";

        // Act
        var response = await _client.GetAsync(requestPath);
        var body = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BookDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(2);
        body!.Select(x => x.Title).Should().Contain(new[] { "Clean Architecture", "Refactoring" });
    }

    [Fact]
    public async Task GetEbooks_ShouldReturnAllEbookCatalogItems()
    {
        // Arrange
        const string requestPath = "/api/ebooks";

        // Act
        var response = await _client.GetAsync(requestPath);
        var body = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<EbookCatalogItemDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(3);
    }

    [Fact]
    public async Task SearchEbooksByName_ShouldReturnMatchingItems()
    {
        // Arrange
        const string requestPath = "/api/ebooks/search?name=Hobbit";

        // Act
        var response = await _client.GetAsync(requestPath);
        var body = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<EbookCatalogItemDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(1);
        body!.Single().Title.Should().Be("The Hobbit");
    }

    [Fact]
    public async Task SearchEbooksByName_ShouldReturnBadRequest_WhenNameIsMissing()
    {
        // Arrange
        const string requestPath = "/api/ebooks/search";

        // Act
        var response = await _client.GetAsync(requestPath);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("name");
    }

    [Fact]
    public async Task GetJournals_ShouldReturnSeededData()
    {
        // Arrange
        const string requestPath = "/api/journals";

        // Act
        var response = await _client.GetAsync(requestPath);
        var body = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<JournalDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(2);
        body!.Select(x => x.Title).Should().Contain(new[] { "IEEE Software", "ACM Queue" });
    }

    [Fact]
    public async Task GetClients_ShouldReturnSeededData()
    {
        // Arrange
        const string requestPath = "/api/clients";

        // Act
        var response = await _client.GetAsync(requestPath);
        var body = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ClientDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body.Should().HaveCount(2);
        body!.Select(x => x.Email).Should().Contain(new[] { "alice.johnson@example.com", "bob.miller@example.com" });
    }

    [Fact]
    public async Task PostBook_ShouldCreateAndReturnBook()
    {
        // Arrange
        var request = new CreateBookRequest("The Pragmatic Programmer", "Andrew Hunt", 1999, "978-0201616224");

        // Act
        var postResponse = await _client.PostAsJsonAsync("/api/books", request);
        var created = await postResponse.Content.ReadFromJsonAsync<BookDto>();

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
        created.Title.Should().Be(request.Title);
        created.Author.Should().Be(request.Author);
        created.PublishedYear.Should().Be(request.PublishedYear);
        created.Isbn.Should().Be(request.Isbn);

        // Act
        var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<BookDto>();

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task PostJournal_ShouldCreateAndReturnJournal()
    {
        // Arrange
        var request = new CreateJournalRequest("Nature", 15, 2026, "Springer");

        // Act
        var postResponse = await _client.PostAsJsonAsync("/api/journals", request);
        var created = await postResponse.Content.ReadFromJsonAsync<JournalDto>();

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
        created.Title.Should().Be(request.Title);
        created.IssueNumber.Should().Be(request.IssueNumber);
        created.PublicationYear.Should().Be(request.PublicationYear);
        created.Publisher.Should().Be(request.Publisher);

        // Act
        var getResponse = await _client.GetAsync($"/api/journals/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<JournalDto>();

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task PostClient_ShouldCreateAndReturnClient()
    {
        // Arrange
        var request = new CreateClientRequest("Charlie", "Brown", "charlie.brown@example.com");

        // Act
        var postResponse = await _client.PostAsJsonAsync("/api/clients", request);
        var created = await postResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
        created.FirstName.Should().Be(request.FirstName);
        created.LastName.Should().Be(request.LastName);
        created.Email.Should().Be(request.Email);

        // Act
        var getResponse = await _client.GetAsync($"/api/clients/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
    }
}
