using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Clients.Commands;
using LibraryService.Domain.Entities;
using Moq;

namespace LibraryService.Tests.Unit.Clients;

public class ClientAddressHandlersTests
{
    [Fact]
    public async Task CreateClientAddressCommand_ShouldCreateAddressAndReturnDto()
    {
        var repository = new Mock<IClientAddressRepository>();
        repository
            .Setup(x => x.AddAsync(It.IsAny<ClientAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientAddress entity, CancellationToken _) => entity);

        var handler = new CreateClientAddressCommandHandler(repository.Object);
        var command = new CreateClientAddressCommand(Guid.NewGuid(), "New York", "USA", "123 Main St", "10001");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBe(Guid.Empty);
        result.City.Should().Be("New York");
        result.Country.Should().Be("USA");
        result.Address.Should().Be("123 Main St");
        result.PostalCode.Should().Be("10001");
    }

    [Fact]
    public async Task CreateClientAddressCommand_ShouldValidateRequiredFields()
    {
        var repository = new Mock<IClientAddressRepository>();
        repository
            .Setup(x => x.AddAsync(It.IsAny<ClientAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientAddress entity, CancellationToken _) => entity);

        var handler = new CreateClientAddressCommandHandler(repository.Object);

        var command = new CreateClientAddressCommand(Guid.NewGuid(), string.Empty, string.Empty, string.Empty, string.Empty);

        var result = await handler.Handle(command, CancellationToken.None);

        result.City.Should().BeEmpty();
        result.Country.Should().BeEmpty();
        result.Address.Should().BeEmpty();
        result.PostalCode.Should().BeEmpty();
    }
}
