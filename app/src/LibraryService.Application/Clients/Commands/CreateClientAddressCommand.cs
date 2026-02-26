using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Clients.Commands;

public record CreateClientAddressCommand(
    Guid ClientId,
    string City,
    string Country,
    string Address,
    string PostalCode) : IRequest<ClientAddressDto?>;

public class CreateClientAddressCommandHandler : IRequestHandler<CreateClientAddressCommand, ClientAddressDto?>
{
    private readonly IClientAddressRepository _repository;

    public CreateClientAddressCommandHandler(IClientAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientAddressDto?> Handle(CreateClientAddressCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _repository.ClientExistsAsync(request.ClientId, cancellationToken);
        if (!clientExists)
        {
            return null;
        }

        var entity = new ClientAddress
        {
            Id = Guid.NewGuid(),
            ClientId = request.ClientId,
            City = request.City,
            Country = request.Country,
            Address = request.Address,
            PostalCode = request.PostalCode,
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return new ClientAddressDto(
            created.Id,
            created.ClientId,
            created.City,
            created.Country,
            created.Address,
            created.PostalCode);
    }
}
