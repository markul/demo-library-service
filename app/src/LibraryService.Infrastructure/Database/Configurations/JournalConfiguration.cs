using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryService.Infrastructure.Database.Configurations;

public class JournalConfiguration : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.ToTable("journals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.IssueNumber)
            .HasColumnName("issue_number")
            .IsRequired();

        builder.Property(x => x.PublicationYear)
            .HasColumnName("publication_year")
            .IsRequired();

        builder.Property(x => x.Publisher)
            .HasColumnName("publisher")
            .HasMaxLength(200)
            .IsRequired();
    }
}
