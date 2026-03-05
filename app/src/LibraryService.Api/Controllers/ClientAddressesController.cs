using LibraryService.Application.ClientAddresses;
using LibraryService.Application.ClientAddresses.Commands;
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

    [HttpPost]
    public async Task<ActionResult<ClientAddressDto>> Create(
        Guid clientId,
        CreateClientAddressRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateClientAddressCommand(
            clientId,
            request.City,
            request.Country,
            request.Address,
            request.PostalCode);

        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { clientId = created.ClientId, id = created.Id },
            created);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<ClientAddressDto> GetById(
        Guid clientId,
        Guid id,
        CancellationToken cancellationToken)
    {
        // This would require a GetClientAddressByIdQuery - for now return NotFound
        return NotFound();
    }
}
