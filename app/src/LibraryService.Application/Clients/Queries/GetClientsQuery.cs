using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Clients.Queries;

public record GetClientsQuery : IRequest<IReadOnlyCollection<ClientDto>>;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IReadOnlyCollection<ClientDto>>
{
    private readonly IClientRepository _repository;

    public GetClientsQueryHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(Map).ToList();
    }

    private static ClientDto Map(Domain.Entities.Client entity)
    {
        return new ClientDto(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.RegisteredAtUtc);
    }
}
