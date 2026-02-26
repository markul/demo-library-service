using FluentAssertions;
using LibraryService.Api.Controllers;
using LibraryService.Application.Status;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Tests.Unit.Status;

public class StatusControllerTests
{
    [Fact]
    public void Get_ShouldReturnOkWithActiveStatus()
    {
        var controller = new StatusController();

        var result = controller.Get();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var payload = okResult.Value.Should().BeOfType<GetStatusResponseDto>().Subject;
        payload.IsActtive.Should().BeTrue();
    }
}
