using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Status.Queries;
using LibraryService.Domain.Entities;
using Moq;

namespace LibraryService.Tests.Unit.Status;

public class GetStatusQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnActive_WhenAnySubscriptionIsActive()
    {
        var repository = new Mock<ISubscriptionRepository>();
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Subscription { IsActive = false },
                new Subscription { IsActive = true },
            });

        var handler = new GetStatusQueryHandler(repository.Object);

        var result = await handler.Handle(new GetStatusQuery(), CancellationToken.None);

        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnInactive_WhenNoSubscriptionsAreActive()
    {
        var repository = new Mock<ISubscriptionRepository>();
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Subscription { IsActive = false },
                new Subscription { IsActive = false },
            });

        var handler = new GetStatusQueryHandler(repository.Object);

        var result = await handler.Handle(new GetStatusQuery(), CancellationToken.None);

        result.IsActive.Should().BeFalse();
    }
}
