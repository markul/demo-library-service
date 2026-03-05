using LibraryService.Application.Status;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/status")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public ActionResult<GetStatusResponseDto> Get()
    {
        return Ok(new GetStatusResponseDto(IsActive: true));
    }
}
