using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Status;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/status")]
public class StatusController : ControllerBase
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public StatusController(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<GetStatusResponseDto>> Get(CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync(cancellationToken);
        var hasActiveSubscription = subscriptions.Any(x => x.IsActive);

        return Ok(new GetStatusResponseDto
        {
            IsActive = hasActiveSubscription
        });
    }
}
