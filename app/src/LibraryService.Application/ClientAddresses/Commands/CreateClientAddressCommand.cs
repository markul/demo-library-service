using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.ClientAddresses.Commands;

public record CreateClientAddressCommand(
    Guid ClientId,
    string City,
    string Country,
    string Address,
    string PostalCode) : IRequest<ClientAddressDto>;

public class CreateClientAddressCommandHandler : IRequestHandler<CreateClientAddressCommand, ClientAddressDto>
{
    private readonly IClientAddressRepository _repository;
    private readonly IClientRepository _clientRepository;

    public CreateClientAddressCommandHandler(
        IClientAddressRepository repository,
        IClientRepository clientRepository)
    {
        _repository = repository;
        _clientRepository = clientRepository;
    }

    public async Task<ClientAddressDto> Handle(CreateClientAddressCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            throw new InvalidOperationException($"Client with ID {request.ClientId} not found.");
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
