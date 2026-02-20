using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Database;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    public DbSet<Journal> Journals => Set<Journal>();

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<SubscriptionType> SubscriptionTypes => Set<SubscriptionType>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<ClientSubscription> ClientSubscriptions => Set<ClientSubscription>();

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
