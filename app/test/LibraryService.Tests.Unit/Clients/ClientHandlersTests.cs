using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
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
        // Arrange
        var repository = new Mock<IClientRepository>();
        repository
            .Setup(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client entity, CancellationToken _) => entity);

        var handler = new CreateClientCommandHandler(repository.Object);
        var command = new CreateClientCommand("Jane", "Doe", "jane.doe@example.com");
        var utcNow = DateTime.UtcNow;

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("jane.doe@example.com");
        result.RegisteredAtUtc.Should().BeCloseTo(utcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetClientsQuery_ShouldMapEntitiesToDtos()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(new GetClientsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.Email).Should().Contain(new[] { "john@example.com", "ann@example.com" });
    }

    [Fact]
    public async Task DeleteClientCommand_ShouldDelegateDeleteToRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repository = new Mock<IClientRepository>();
        repository
            .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteClientCommandHandler(repository.Object);

        // Act
        var deleted = await handler.Handle(new DeleteClientCommand(id), CancellationToken.None);

        // Assert
        deleted.Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
