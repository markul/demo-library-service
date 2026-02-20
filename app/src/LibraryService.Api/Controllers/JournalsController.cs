using LibraryService.Application.Journals;
using LibraryService.Application.Journals.Commands;
using LibraryService.Application.Journals.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/journals")]
public class JournalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JournalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<JournalDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetJournalsQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JournalDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetJournalByIdQuery(id), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<JournalDto>> Create(CreateJournalRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateJournalCommand(request.Title, request.IssueNumber, request.PublicationYear, request.Publisher);
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateJournalRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateJournalCommand(id, request.Title, request.IssueNumber, request.PublicationYear, request.Publisher);
        var updated = await _mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteJournalCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
