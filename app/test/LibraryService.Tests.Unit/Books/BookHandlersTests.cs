using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Books.Commands;
using LibraryService.Application.Books.Queries;
using LibraryService.Domain.Entities;
using Moq;

namespace LibraryService.Tests.Unit.Books;

public class BookHandlersTests
{
    [Fact]
    public async Task CreateBookCommand_ShouldCreateBookAndReturnDto_WhenRequestIsValid()
    {
        var repository = new Mock<IBookRepository>();
        repository
            .Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book entity, CancellationToken _) => entity);

        var handler = new CreateBookCommandHandler(repository.Object);
        var command = new CreateBookCommand("Clean Code", "Robert C. Martin", 2008, "978-0132350884");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBe(Guid.Empty);
        result.Title.Should().Be("Clean Code");
        result.Author.Should().Be("Robert C. Martin");
        result.PublishedYear.Should().Be(2008);
        result.Isbn.Should().Be("978-0132350884");
        repository.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookCommand_ShouldReturnFalse_WhenBookDoesNotExist()
    {
        var repository = new Mock<IBookRepository>();
        repository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var handler = new UpdateBookCommandHandler(repository.Object);
        var command = new UpdateBookCommand(Guid.NewGuid(), "Refactoring", "Martin Fowler", 1999, "978-0201485677");

        var updated = await handler.Handle(command, CancellationToken.None);

        updated.Should().BeFalse();
        repository.Verify(x => x.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetBooksQuery_ShouldMapEntitiesToDtos()
    {
        var books = new List<Book>
        {
            new() { Id = Guid.NewGuid(), Title = "DDD", Author = "Eric Evans", PublishedYear = 2003, Isbn = "978-0321125217" },
            new() { Id = Guid.NewGuid(), Title = "Patterns", Author = "GoF", PublishedYear = 1994, Isbn = "978-0201633610" },
        };

        var repository = new Mock<IBookRepository>();
        repository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var handler = new GetBooksQueryHandler(repository.Object);

        var result = await handler.Handle(new GetBooksQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(x => x.Title).Should().Contain(new[] { "DDD", "Patterns" });
    }

    [Fact]
    public async Task DeleteBookCommand_ShouldDelegateDeleteToRepository()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IBookRepository>();
        repository
            .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteBookCommandHandler(repository.Object);

        var deleted = await handler.Handle(new DeleteBookCommand(id), CancellationToken.None);

        deleted.Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
