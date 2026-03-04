using FluentAssertions;
using LibraryService.Application.ClientAddresses;
using LibraryService.Application.Clients;
using LibraryService.Tests.Integration.Controllers;
using LibraryService.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace LibraryService.Tests.Integration.Controllers;

public class ClientAddressesControllerIntegrationTests : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client;

    public ClientAddressesControllerIntegrationTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostClientAddress_ShouldCreateAndReturnAddress()
    {
        // First create a client to associate the address with
        var clientRequest = new CreateClientRequest("Test", "User", "test@example.com");
        var postClientResponse = await _client.PostAsJsonAsync("/api/clients", clientRequest);
        postClientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdClient = await postClientResponse.Content.ReadFromJsonAsync<ClientDto>();
        createdClient.Should().NotBeNull();
        createdClient!.Id.Should().NotBe(Guid.Empty);

        // Now create an address for the client
        var request = new CreateClientAddressRequest(
            "New York",
            "USA",
            "123 Main St",
            "10001");

        var postResponse = await _client.PostAsJsonAsync($"/api/clients/{createdClient.Id}/addresses", request);
        var created = await postResponse.Content.ReadFromJsonAsync<ClientAddressDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
        created.ClientId.Should().Be(createdClient.Id);
        created.City.Should().Be(request.City);
        created.Country.Should().Be(request.Country);
        created.Address.Should().Be(request.Address);
        created.PostalCode.Should().Be(request.PostalCode);

        // Verify we can retrieve the address
        var getResponse = await _client.GetAsync($"/api/clients/{createdClient.Id}/addresses");
        var fetched = await getResponse.Content.ReadFromJsonAsync<ClientAddressDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetClientAddress_ShouldReturnAddress_WhenExists()
    {
        // First create a client
        var clientRequest = new CreateClientRequest("Test", "User", "test2@example.com");
        var postClientResponse = await _client.PostAsJsonAsync("/api/clients", clientRequest);
        postClientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdClient = await postClientResponse.Content.ReadFromJsonAsync<ClientDto>();
        createdClient.Should().NotBeNull();
        createdClient!.Id.Should().NotBe(Guid.Empty);

        // Create an address for the client
        var addressRequest = new CreateClientAddressRequest(
            "New York",
            "USA",
            "123 Main St",
            "10001");
        
        var postAddressResponse = await _client.PostAsJsonAsync($"/api/clients/{createdClient.Id}/addresses", addressRequest);
        postAddressResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAddress = await postAddressResponse.Content.ReadFromJsonAsync<ClientAddressDto>();
        createdAddress.Should().NotBeNull();
        createdAddress!.Id.Should().NotBe(Guid.Empty);

        // Retrieve the address
        var getResponse = await _client.GetAsync($"/api/clients/{createdClient.Id}/addresses");
        var fetched = await getResponse.Content.ReadFromJsonAsync<ClientAddressDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(createdAddress.Id);
    }

    [Fact]
    public async Task GetClientAddress_ShouldReturnNotFound_WhenNoAddressExists()
    {
        // Create a client without an address
        var clientRequest = new CreateClientRequest("Test", "User2", "test3@example.com");
        var postClientResponse = await _client.PostAsJsonAsync("/api/clients", clientRequest);
        postClientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdClient = await postClientResponse.Content.ReadFromJsonAsync<ClientDto>();
        createdClient.Should().NotBeNull();
        createdClient!.Id.Should().NotBe(Guid.Empty);

        // Try to retrieve address (should return 404)
        var getResponse = await _client.GetAsync($"/api/clients/{createdClient.Id}/addresses");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostClientAddress_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Create a client
        var clientRequest = new CreateClientRequest("Test", "User3", "test4@example.com");
        var postClientResponse = await _client.PostAsJsonAsync("/api/clients", clientRequest);
        postClientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdClient = await postClientResponse.Content.ReadFromJsonAsync<ClientDto>();
        createdClient.Should().NotBeNull();
        createdClient!.Id.Should().NotBe(Guid.Empty);

        // Try to create address with invalid data (empty city)
        var invalidRequest = new CreateClientAddressRequest(
            "",
            "USA",
            "123 Main St",
            "10001");

        var postResponse = await _client.PostAsJsonAsync($"/api/clients/{createdClient.Id}/addresses", invalidRequest);
        postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
