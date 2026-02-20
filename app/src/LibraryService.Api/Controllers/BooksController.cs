using LibraryService.Application.Books;
using LibraryService.Application.Books.Commands;
using LibraryService.Application.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<BookDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetBooksQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetBookByIdQuery(id), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(CreateBookRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(request.Title, request.Author, request.PublishedYear, request.Isbn);
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateBookCommand(id, request.Title, request.Author, request.PublishedYear, request.Isbn);
        var updated = await _mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteBookCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
