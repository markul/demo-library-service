namespace LibraryService.Application.Clients;

public record ClientDto(Guid Id, string FirstName, string LastName, string Email, DateTime RegisteredAtUtc);

public record CreateClientRequest(string FirstName, string LastName, string Email);

public record UpdateClientRequest(string FirstName, string LastName, string Email);
