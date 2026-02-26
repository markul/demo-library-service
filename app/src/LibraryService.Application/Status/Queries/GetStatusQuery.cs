using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Status.Queries;

public record GetStatusQuery : IRequest<GetStatusResponseDto>;

public class GetStatusQueryHandler : IRequestHandler<GetStatusQuery, GetStatusResponseDto>
{
    private readonly ISubscriptionRepository _repository;

    public GetStatusQueryHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetStatusResponseDto> Handle(GetStatusQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _repository.GetAllAsync(cancellationToken);

        return new GetStatusResponseDto
        {
            IsActive = subscriptions.Any(x => x.IsActive)
        };
    }
}
