using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.ClientAddresses.Queries;

public record GetClientAddressByClientIdQuery(Guid ClientId) : IRequest<ClientAddressDto?>;

public class GetClientAddressByClientIdQueryHandler : IRequestHandler<GetClientAddressByClientIdQuery, ClientAddressDto?>
{
    private readonly IClientAddressRepository _repository;

    public GetClientAddressByClientIdQueryHandler(IClientAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientAddressDto?> Handle(GetClientAddressByClientIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByClientIdAsync(request.ClientId, cancellationToken);
        return entity is null 
            ? null
            : new ClientAddressDto(
                entity.Id,
                entity.ClientId,
                entity.City,
                entity.Country,
                entity.Address,
                entity.PostalCode);
    }
}
