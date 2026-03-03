using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Status;
using MediatR;

namespace LibraryService.Application.Status.Queries;

public record GetStatusQuery : IRequest<GetStatusResponseDto>;

public class GetStatusQueryHandler : IRequestHandler<GetStatusQuery, GetStatusResponseDto>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetStatusQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<GetStatusResponseDto> Handle(GetStatusQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync(cancellationToken);
        var isActive = subscriptions.Any(s => s.IsActive);
        return new GetStatusResponseDto(IsActive: isActive);
    }
}
