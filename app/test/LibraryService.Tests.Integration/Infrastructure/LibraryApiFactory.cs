using LibraryService.Application.Abstractions.Services;
using LibraryService.Application.Ebooks;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LibraryService.Tests.Integration.Infrastructure;

public class LibraryApiFactory : WebApplicationFactory<Program>
{
    private readonly InMemoryDatabaseRoot _databaseRoot = new();
    private readonly string _databaseName = $"library-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<LibraryDbContext>));
            services.RemoveAll(typeof(LibraryDbContext));
            services.RemoveAll<IEbookCatalogService>();

            services.AddDbContext<LibraryDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName, _databaseRoot));
            services.AddScoped<IEbookCatalogService, FakeEbookCatalogService>();
        });
    }

    public void ResetAndSeed()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        Seed(dbContext);
    }

    private static void Seed(LibraryDbContext dbContext)
    {
        var now = DateTime.UtcNow;

        dbContext.Books.AddRange(
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Clean Architecture",
                Author = "Robert C. Martin",
                PublishedYear = 2017,
                Isbn = "978-0134494166",
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Refactoring",
                Author = "Martin Fowler",
                PublishedYear = 2018,
                Isbn = "978-0134757599",
            });

        dbContext.Journals.AddRange(
            new Journal
            {
                Id = Guid.NewGuid(),
                Title = "IEEE Software",
                IssueNumber = 4,
                PublicationYear = 2026,
                Publisher = "IEEE",
            },
            new Journal
            {
                Id = Guid.NewGuid(),
                Title = "ACM Queue",
                IssueNumber = 2,
                PublicationYear = 2026,
                Publisher = "ACM",
            });

        dbContext.Clients.AddRange(
            new Client
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com",
                RegisteredAtUtc = now.AddDays(-7),
            },
            new Client
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Miller",
                Email = "bob.miller@example.com",
                RegisteredAtUtc = now.AddDays(-1),
            });

        dbContext.SaveChanges();
    }

    private sealed class FakeEbookCatalogService : IEbookCatalogService
    {
        private static readonly IReadOnlyCollection<EbookCatalogItemDto> Books =
        [
            new(1, "Dune", "Frank Herbert", "Science Fiction", 15.50m, 1965, "English"),
            new(2, "The Hobbit", "J.R.R. Tolkien", "Fantasy", 13.75m, 1937, "English"),
            new(3, "The Name of the Wind", "Patrick Rothfuss", "Fantasy", 14.20m, 2007, "English")
        ];

        public Task<IReadOnlyCollection<EbookCatalogItemDto>> GetBooksAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Books);

        public Task<IReadOnlyCollection<EbookCatalogItemDto>> FindBooksByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var books = Books
                .Where(x => x.Title.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<EbookCatalogItemDto>>(books);
        }
    }
}
