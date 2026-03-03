namespace LibraryService.Application.Clients;

public record ClientAddressDto(
    Guid Id,
    Guid ClientId,
    string City,
    string Country,
    string Address,
    string PostalCode);

public record CreateClientAddressRequest(
    Guid ClientId,
    string City,
    string Country,
    string Address,
    string PostalCode);

public record UpdateClientAddressRequest(
    string City,
    string Country,
    string Address,
    string PostalCode);
