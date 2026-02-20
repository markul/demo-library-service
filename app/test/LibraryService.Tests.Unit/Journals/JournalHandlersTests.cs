using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Journals.Commands;
using LibraryService.Application.Journals.Queries;
using LibraryService.Domain.Entities;
using Moq;

namespace LibraryService.Tests.Unit.Journals;

public class JournalHandlersTests
{
    [Fact]
    public async Task CreateJournalCommand_ShouldCreateJournalAndReturnDto()
    {
        var repository = new Mock<IJournalRepository>();
        repository
            .Setup(x => x.AddAsync(It.IsAny<Journal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Journal entity, CancellationToken _) => entity);

        var handler = new CreateJournalCommandHandler(repository.Object);
        var command = new CreateJournalCommand("ACM Journal", 12, 2026, "ACM");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBe(Guid.Empty);
        result.Title.Should().Be("ACM Journal");
        result.IssueNumber.Should().Be(12);
        result.PublicationYear.Should().Be(2026);
        result.Publisher.Should().Be("ACM");
    }

    [Fact]
    public async Task GetJournalByIdQuery_ShouldReturnNull_WhenJournalNotFound()
    {
        var repository = new Mock<IJournalRepository>();
        repository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Journal?)null);

        var handler = new GetJournalByIdQueryHandler(repository.Object);

        var result = await handler.Handle(new GetJournalByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateJournalCommand_ShouldUpdateAndReturnTrue_WhenJournalExists()
    {
        var id = Guid.NewGuid();
        var journal = new Journal
        {
            Id = id,
            Title = "Old",
            IssueNumber = 1,
            PublicationYear = 2020,
            Publisher = "Old Publisher",
        };

        var repository = new Mock<IJournalRepository>();
        repository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(journal);
        repository
            .Setup(x => x.UpdateAsync(journal, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateJournalCommandHandler(repository.Object);
        var command = new UpdateJournalCommand(id, "New", 2, 2026, "New Publisher");

        var updated = await handler.Handle(command, CancellationToken.None);

        updated.Should().BeTrue();
        journal.Title.Should().Be("New");
        journal.IssueNumber.Should().Be(2);
        journal.PublicationYear.Should().Be(2026);
        journal.Publisher.Should().Be("New Publisher");
    }
}
