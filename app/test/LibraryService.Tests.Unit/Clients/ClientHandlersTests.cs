using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Clients;
using LibraryService.Application.Clients.Commands;
using LibraryService.Application.Clients.Queries;
using LibraryService.Domain.Entities;
using Moq;

namespace LibraryService.Tests.Unit.Clients;

public class ClientHandlersTests
{
    [Fact]
    public async Task CreateClientCommand_ShouldCreateClientAndReturnDto()
    {
        var repository = new Mock<IClientRepository>();
        repository
            .Setup(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client entity, CancellationToken _) => entity);

        var handler = new CreateClientCommandHandler(repository.Object);
        var command = new CreateClientCommand("Jane", "Doe", "jane.doe@example.com");
        var utcNow = DateTime.UtcNow;

        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBe(Guid.Empty);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("jane.doe@example.com");
        result.RegisteredAtUtc.Should().BeCloseTo(utcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetClientsQuery_ShouldMapEntitiesToDtos()
    {
        var clients = new List<Client>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com", RegisteredAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), FirstName = "Ann", LastName = "Smith", Email = "ann@example.com", RegisteredAtUtc = DateTime.UtcNow },
        };

        var repository = new Mock<IClientRepository>();
        repository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clients);

        var handler = new GetClientsQueryHandler(repository.Object);

        var result = await handler.Handle(new GetClientsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(x => x.Email).Should().Contain(new[] { "john@example.com", "ann@example.com" });
    }

    [Fact]
    public async Task DeleteClientCommand_ShouldDelegateDeleteToRepository()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IClientRepository>();
        repository
            .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteClientCommandHandler(repository.Object);

        var deleted = await handler.Handle(new DeleteClientCommand(id), CancellationToken.None);

        deleted.Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateClientAddressCommand_ShouldCreateAddressAndReturnDto_WhenClientExists()
    {
        var clientId = Guid.NewGuid();
        var repository = new Mock<IClientAddressRepository>();
        repository
            .Setup(x => x.ClientExistsAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repository
            .Setup(x => x.AddAsync(It.IsAny<ClientAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientAddress entity, CancellationToken _) => entity);

        var handler = new CreateClientAddressCommandHandler(repository.Object);
        var command = new CreateClientAddressCommand(
            clientId,
            "Seattle",
            "USA",
            "1st Ave 100",
            "98101");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().NotBe(Guid.Empty);
        result.ClientId.Should().Be(clientId);
        result.City.Should().Be("Seattle");
        result.Country.Should().Be("USA");
        result.Address.Should().Be("1st Ave 100");
        result.PostalCode.Should().Be("98101");
    }

    [Fact]
    public async Task CreateClientAddressCommand_ShouldReturnNull_WhenClientDoesNotExist()
    {
        var repository = new Mock<IClientAddressRepository>();
        repository
            .Setup(x => x.ClientExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CreateClientAddressCommandHandler(repository.Object);
        var command = new CreateClientAddressCommand(
            Guid.NewGuid(),
            "Seattle",
            "USA",
            "1st Ave 100",
            "98101");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        repository.Verify(x => x.AddAsync(It.IsAny<ClientAddress>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
