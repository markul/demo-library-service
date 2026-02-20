using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.Clients.Commands;

public record CreateClientCommand(string FirstName, string LastName, string Email) : IRequest<ClientDto>;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _repository;

    public CreateClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            RegisteredAtUtc = DateTime.UtcNow,
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return new ClientDto(created.Id, created.FirstName, created.LastName, created.Email, created.RegisteredAtUtc);
    }
}
