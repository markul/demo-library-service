using LibraryService.Application.Subscriptions;
using LibraryService.Application.Subscriptions.Commands;
using LibraryService.Application.Subscriptions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SubscriptionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetSubscriptionsQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubscriptionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetSubscriptionByIdQuery(id), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create(CreateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateSubscriptionCommand(
            request.Name,
            request.SubscriptionTypeId,
            request.IsActive,
            request.StartDateUtc,
            request.ClientIds);
        var created = await _mediator.Send(command, cancellationToken);
        if (created is null)
        {
            return NotFound("Subscription type or one of clients was not found.");
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSubscriptionCommand(
            id,
            request.Name,
            request.SubscriptionTypeId,
            request.IsActive,
            request.StartDateUtc,
            request.ClientIds);
        var updated = await _mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSubscriptionCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyCollection<SubscriptionTypeDto>>> GetTypes(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetSubscriptionTypesQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("types/{id:guid}")]
    public async Task<ActionResult<SubscriptionTypeDto>> GetTypeById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetSubscriptionTypeByIdQuery(id), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("types")]
    public async Task<ActionResult<SubscriptionTypeDto>> CreateType(CreateSubscriptionTypeRequest request, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateSubscriptionTypeCommand(request.Name, request.Period, request.Price), cancellationToken);
        return CreatedAtAction(nameof(GetTypeById), new { id = created.Id }, created);
    }

    [HttpPut("types/{id:guid}")]
    public async Task<IActionResult> UpdateType(Guid id, UpdateSubscriptionTypeRequest request, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateSubscriptionTypeCommand(id, request.Name, request.Period, request.Price), cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("types/{id:guid}")]
    public async Task<IActionResult> DeleteType(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSubscriptionTypeCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var command = new CheckoutSubscriptionCommand(
            request.ClientId,
            request.SubscriptionTypeId,
            request.IdempotencyKey);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Response is null)
        {
            return result.Outcome switch
            {
                CheckoutSubscriptionOutcome.NotFound => NotFound("Client or subscription type was not found."),
                CheckoutSubscriptionOutcome.Conflict => Conflict("Idempotency key is already used with a different request payload."),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            };
        }

        return result.Outcome switch
        {
            CheckoutSubscriptionOutcome.Created => CreatedAtAction(
                nameof(GetById),
                new { id = result.Response.SubscriptionId },
                result.Response),
            CheckoutSubscriptionOutcome.Ok => Ok(result.Response),
            CheckoutSubscriptionOutcome.Accepted => Accepted(result.Response),
            CheckoutSubscriptionOutcome.PaymentRequired => StatusCode(
                StatusCodes.Status402PaymentRequired,
                result.Response),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }
}
