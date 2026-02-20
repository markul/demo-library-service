using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Clients.Commands;

public record UpdateClientCommand(Guid Id, string FirstName, string LastName, string Email) : IRequest<bool>;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, bool>
{
    private readonly IClientRepository _repository;

    public UpdateClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.FirstName = request.FirstName;
        existing.LastName = request.LastName;
        existing.Email = request.Email;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }
}
