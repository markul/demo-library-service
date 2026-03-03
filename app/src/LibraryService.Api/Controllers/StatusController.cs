using LibraryService.Application.Status;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetStatusResponseDto>> Get(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetStatusQuery(), cancellationToken);
        return Ok(response);
    }
}

