using Microsoft.AspNetCore.Mvc;
using LibraryService.Api.Models;

namespace LibraryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var response = new GetStatusResponseDto { IsActive = true };
            return Ok(response);
        }
    }
}
