namespace LibraryService.Application.Clients;

public record ClientAddressDto(
    Guid Id,
    Guid ClientId,
    string City,
    string Country,
    string Address,
    string PostalCode);

public record CreateClientAddressRequest(string City, string Country, string Address, string PostalCode);
