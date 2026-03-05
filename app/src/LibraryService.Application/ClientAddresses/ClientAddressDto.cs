namespace LibraryService.Application.ClientAddresses;

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
