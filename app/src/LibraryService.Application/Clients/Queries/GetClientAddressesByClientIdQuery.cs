using LibraryService.Application.Clients;
using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Clients.Queries;

public record GetClientAddressesByClientIdQuery(Guid ClientId) : IRequest<IReadOnlyCollection<ClientAddressDto>>;

public class GetClientAddressesByClientIdQueryHandler : IRequestHandler<GetClientAddressesByClientIdQuery, IReadOnlyCollection<ClientAddressDto>>
{
    private readonly IClientAddressRepository _repository;

    public GetClientAddressesByClientIdQueryHandler(IClientAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ClientAddressDto>> Handle(GetClientAddressesByClientIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities
            .Where(x => x.ClientId == request.ClientId)
            .Select(entity => new ClientAddressDto(
                entity.Id,
                entity.ClientId,
                entity.City,
                entity.Country,
                entity.Address,
                entity.PostalCode))
            .ToList();
    }
}
