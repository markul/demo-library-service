using LibraryService.Application.Clients;
using LibraryService.Application.Clients.Commands;
using LibraryService.Application.Clients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ClientDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetClientsQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetClientByIdQuery(id), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateClientCommand(request.FirstName, request.LastName, request.Email);
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClientRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateClientCommand(id, request.FirstName, request.LastName, request.Email);
        var updated = await _mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteClientCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{clientId:guid}/addresses")]
    public async Task<ActionResult<ClientAddressDto>> CreateAddress(Guid clientId, CreateClientAddressRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateClientAddressCommand(new CreateClientAddressRequest(clientId, request.City, request.Country, request.Address, request.PostalCode));
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAddressById), new { clientId = clientId, id = created.Id }, created);
    }

    [HttpGet("{clientId:guid}/addresses/{id:guid}")]
    public async Task<ActionResult<ClientAddressDto>> GetAddressById(Guid clientId, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetClientAddressByIdQuery(clientId, id);
        var item = await _mediator.Send(query, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{clientId:guid}/addresses")]
    public async Task<ActionResult<IReadOnlyCollection<ClientAddressDto>>> GetAddressesByClientId(Guid clientId, CancellationToken cancellationToken)
    {
        var query = new GetClientAddressesByClientIdQuery(clientId);
        var items = await _mediator.Send(query, cancellationToken);
        return Ok(items);
    }
}

