using FluentAssertions;
using LibraryService.Api.Controllers;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Status;
using LibraryService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryService.Tests.Unit.Status;

public class StatusControllerTests
{
    [Fact]
    public async Task Get_ShouldReturnOkWithActiveStatus_WhenActiveSubscriptionExists()
    {
        var repository = new Mock<ISubscriptionRepository>();
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Subscription { IsActive = true } });

        var controller = new StatusController(repository.Object);

        var result = await controller.Get(CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var payload = okResult.Value.Should().BeOfType<GetStatusResponseDto>().Subject;
        payload.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Get_ShouldReturnOkWithInactiveStatus_WhenNoActiveSubscriptionExists()
    {
        var repository = new Mock<ISubscriptionRepository>();
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Subscription { IsActive = false } });

        var controller = new StatusController(repository.Object);

        var result = await controller.Get(CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var payload = okResult.Value.Should().BeOfType<GetStatusResponseDto>().Subject;
        payload.IsActive.Should().BeFalse();
    }
}
