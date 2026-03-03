using LibraryService.Application.Clients;
using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Clients.Queries;

public record GetClientAddressByIdQuery(Guid ClientId, Guid Id) : IRequest<ClientAddressDto?>;

public class GetClientAddressByIdQueryHandler : IRequestHandler<GetClientAddressByIdQuery, ClientAddressDto?>
{
    private readonly IClientAddressRepository _repository;

    public GetClientAddressByIdQueryHandler(IClientAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientAddressDto?> Handle(GetClientAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        return new ClientAddressDto(
            entity.Id,
            entity.ClientId,
            entity.City,
            entity.Country,
            entity.Address,
            entity.PostalCode);
    }
}
