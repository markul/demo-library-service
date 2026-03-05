using LibraryService.Application.Abstractions.Repositories;

namespace LibraryService.Application.Status;

public class StatusService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public StatusService(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<GetStatusResponseDto> GetStatusAsync(CancellationToken cancellationToken)
    {
        var hasActiveSubscriptions = await _subscriptionRepository.HasActiveSubscriptionsAsync(cancellationToken);
        return new GetStatusResponseDto(IsActive: hasActiveSubscriptions);
    }
}
