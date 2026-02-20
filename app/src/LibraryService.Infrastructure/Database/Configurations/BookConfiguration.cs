using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryService.Infrastructure.Database.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Author)
            .HasColumnName("author")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PublishedYear)
            .HasColumnName("published_year")
            .IsRequired();

        builder.Property(x => x.Isbn)
            .HasColumnName("isbn")
            .HasMaxLength(64)
            .IsRequired();
    }
}
