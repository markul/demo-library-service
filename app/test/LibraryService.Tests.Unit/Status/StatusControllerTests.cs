using FluentAssertions;
using LibraryService.Api.Controllers;
using LibraryService.Application.Status;
using LibraryService.Application.Status.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryService.Tests.Unit.Status;

public class StatusControllerTests
{
    [Fact]
    public async Task Get_ShouldReturnOkWithPayloadFromMediator()
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(x => x.Send(It.IsAny<GetStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetStatusResponseDto { IsActive = true });

        var controller = new StatusController(mediator.Object);

        var result = await controller.Get(CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var payload = okResult.Value.Should().BeOfType<GetStatusResponseDto>().Subject;
        payload.IsActive.Should().BeTrue();
        mediator.Verify(x => x.Send(It.IsAny<GetStatusQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
