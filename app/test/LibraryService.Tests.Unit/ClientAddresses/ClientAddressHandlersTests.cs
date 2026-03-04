using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.ClientAddresses.Commands;
using LibraryService.Application.ClientAddresses.Queries;
using LibraryService.Domain.Entities;
using Moq;

namespace LibraryService.Tests.Unit.ClientAddresses;

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
        var command = new CreateClientAddressCommand(
            Guid.NewGuid(),
            "New York",
            "USA",
            "123 Main St",
            "10001");
        
        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBe(Guid.Empty);
        result.City.Should().Be("New York");
        result.Country.Should().Be("USA");
        result.Address.Should().Be("123 Main St");
        result.PostalCode.Should().Be("10001");
    }

    [Fact]
    public async Task GetClientAddressByClientIdQuery_ShouldReturnAddress_WhenExists()
    {
        var clientId = Guid.NewGuid();
        var address = new ClientAddress
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            City = "New York",
            Country = "USA",
            Address = "123 Main St",
            PostalCode = "10001"
        };

        var repository = new Mock<IClientAddressRepository>();
        repository
            .Setup(x => x.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        var handler = new GetClientAddressByClientIdQueryHandler(repository.Object);

        var result = await handler.Handle(new GetClientAddressByClientIdQuery(clientId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.City.Should().Be("New York");
        result.Country.Should().Be("USA");
        result.Address.Should().Be("123 Main St");
        result.PostalCode.Should().Be("10001");
        repository.Verify(x => x.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetClientAddressByClientIdQuery_ShouldReturnNull_WhenNotFound()
    {
        var clientId = Guid.NewGuid();
        var repository = new Mock<IClientAddressRepository>();
        repository
            .Setup(x => x.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientAddress?)null);

        var handler = new GetClientAddressByClientIdQueryHandler(repository.Object);

        var result = await handler.Handle(new GetClientAddressByClientIdQuery(clientId), CancellationToken.None);

        result.Should().BeNull();
        repository.Verify(x => x.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
