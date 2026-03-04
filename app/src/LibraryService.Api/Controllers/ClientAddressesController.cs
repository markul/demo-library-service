using LibraryService.Application.ClientAddresses;
using LibraryService.Application.ClientAddresses.Commands;
using LibraryService.Application.ClientAddresses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/clients/{clientId:guid}/addresses")]
public class ClientAddressesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientAddressesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ClientAddressDto?>> GetByClientId(Guid clientId, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetClientAddressByClientIdQuery(clientId), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ClientAddressDto>> Create(Guid clientId, CreateClientAddressRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateClientAddressCommand(
            clientId,
            request.City,
            request.Country,
            request.Address,
            request.PostalCode);
        
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetByClientId), new { clientId = created.ClientId }, created);
    }
}

