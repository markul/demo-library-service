using LibraryService.Application.Payments;
using LibraryService.Application.Payments.Commands;
using LibraryService.Application.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PaymentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetPaymentsQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetPaymentByIdQuery(id), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(
            request.UniqueId,
            request.Amount,
            request.SubscriptionId,
            request.ClientId,
            request.ExternalId,
            request.Status);
        var created = await _mediator.Send(command, cancellationToken);
        if (created is null)
        {
            return BadRequest("Invalid client/subscription reference or duplicate unique id.");
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePaymentCommand(
            id,
            request.UniqueId,
            request.Amount,
            request.SubscriptionId,
            request.ClientId,
            request.ExternalId,
            request.Status);
        var updated = await _mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeletePaymentCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
