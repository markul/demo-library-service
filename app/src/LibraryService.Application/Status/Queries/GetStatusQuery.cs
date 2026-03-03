using MediatR;
using LibraryService.Application.Status;

namespace LibraryService.Application.Status.Queries;

public record GetStatusQuery : IRequest<GetStatusResponseDto>;

public class GetStatusQueryHandler : IRequestHandler<GetStatusQuery, GetStatusResponseDto>
{
    public Task<GetStatusResponseDto> Handle(GetStatusQuery request, CancellationToken cancellationToken)
    {
        // For now, return a fixed status. In production, you might check database connectivity,
        // external service health, etc.
        return Task.FromResult(new GetStatusResponseDto(IsActive: true));
    }
}