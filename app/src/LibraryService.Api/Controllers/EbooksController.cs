using LibraryService.Application.Ebooks;
using LibraryService.Application.Ebooks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/ebooks")]
public class EbooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<EbookCatalogItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var items = await mediator.Send(new GetEbookCatalogQuery(), cancellationToken);
            return Ok(items);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, "Ebook service request failed.");
        }
        catch (JsonException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, "Ebook service returned an invalid response.");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyCollection<EbookCatalogItemDto>>> SearchByName(
        [FromQuery] string? name,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Query parameter 'name' is required.");
        }

        try
        {
            var items = await mediator.Send(new GetEbookCatalogByNameQuery(name), cancellationToken);
            return Ok(items);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, "Ebook service request failed.");
        }
        catch (JsonException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, "Ebook service returned an invalid response.");
        }
    }
}

