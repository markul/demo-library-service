using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EbookService.Tests;

public class BooksODataTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetBooks_ReturnsOneHundredBooks()
    {
        var response = await _client.GetAsync("/odata/Books");
        response.EnsureSuccessStatusCode();

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var books = body.RootElement.GetProperty("value");

        Assert.Equal(100, books.GetArrayLength());
    }

    [Fact]
    public async Task GetBooks_FilteredByGenre_ReturnsOnlyFantasyBooks()
    {
        var response = await _client.GetAsync("/odata/Books?$filter=Genre%20eq%20'Fantasy'");
        response.EnsureSuccessStatusCode();

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var books = body.RootElement.GetProperty("value").EnumerateArray();

        var count = 0;
        foreach (var book in books)
        {
            count++;
            Assert.Equal("Fantasy", book.GetProperty("Genre").GetString());
        }

        Assert.Equal(10, count);
    }
}
