using LibraryService.Application.Status;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public ActionResult<GetStatusResponseDto> Get()
    {
        var response = new GetStatusResponseDto(true);
        return Ok(response);
    }
}