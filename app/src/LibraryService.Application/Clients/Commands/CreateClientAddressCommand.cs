using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Clients.Commands;

public record CreateClientAddressCommand(CreateClientAddressRequest Request) : IRequest<ClientAddressDto>;

public class CreateClientAddressCommandHandler : IRequestHandler<CreateClientAddressCommand, ClientAddressDto>
{
    private readonly IClientRepository _clientRepository;
    private readonly IClientAddressRepository _clientAddressRepository;

    public CreateClientAddressCommandHandler(
        IClientRepository clientRepository,
        IClientAddressRepository clientAddressRepository)
    {
        _clientRepository = clientRepository;
        _clientAddressRepository = clientAddressRepository;
    }

    public async Task<ClientAddressDto> Handle(CreateClientAddressCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            throw new ArgumentException($"Client with ID {request.Request.ClientId} not found.");
        }

        var entity = new ClientAddress
        {
            Id = Guid.NewGuid(),
            ClientId = request.Request.ClientId,
            City = request.Request.City,
            Country = request.Request.Country,
            Address = request.Request.Address,
            PostalCode = request.Request.PostalCode
        };

        var created = await _clientAddressRepository.AddAsync(entity, cancellationToken);
        return new ClientAddressDto(
            created.Id,
            created.ClientId,
            created.City,
            created.Country,
            created.Address,
            created.PostalCode);
    }
}
