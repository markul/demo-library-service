using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryService.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedRealisticLibraryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
DELETE FROM clients
WHERE email LIKE 'seed.client.%@example.com';
");

            migrationBuilder.Sql(
                @"
DELETE FROM books
WHERE isbn LIKE 'SEED-ISBN-%'
   OR title LIKE 'Seed Book %';
");

            migrationBuilder.Sql(
                @"
DELETE FROM journals
WHERE title LIKE 'Seed Journal %'
   OR publisher LIKE 'Seed Publisher %';
");

            migrationBuilder.Sql(
                @"
WITH data AS (
    SELECT
        ARRAY[
            'Olivia','Liam','Emma','Noah','Ava','Elijah','Sophia','Lucas','Isabella','Mason',
            'Mia','Logan','Charlotte','James','Amelia','Benjamin','Harper','Ethan','Evelyn','Henry'
        ]::text[] AS first_names,
        ARRAY[
            'Smith','Johnson','Williams','Brown','Jones','Garcia','Miller','Davis','Rodriguez','Martinez',
            'Hernandez','Lopez','Gonzalez','Wilson','Anderson','Thomas','Taylor','Moore','Jackson','Martin'
        ]::text[] AS last_names
)
INSERT INTO clients (id, first_name, last_name, email, registered_at_utc)
SELECT
    (
        substr(md5('real-client-' || i::text), 1, 8) || '-' ||
        substr(md5('real-client-' || i::text), 9, 4) || '-' ||
        substr(md5('real-client-' || i::text), 13, 4) || '-' ||
        substr(md5('real-client-' || i::text), 17, 4) || '-' ||
        substr(md5('real-client-' || i::text), 21, 12)
    )::uuid,
    n.first_name,
    n.last_name,
    lower(n.first_name || '.' || n.last_name || i::text || '@library.local'),
    now() - (i::text || ' days')::interval
FROM generate_series(1, 100) AS s(i)
CROSS JOIN data d
CROSS JOIN LATERAL (
    SELECT
        d.first_names[1 + ((i - 1) % array_length(d.first_names, 1))] AS first_name,
        d.last_names[1 + (((i - 1) / array_length(d.first_names, 1)) % array_length(d.last_names, 1))] AS last_name
) AS n;
");

            migrationBuilder.Sql(
                @"
WITH data AS (
    SELECT
        ARRAY[
            'Clean Code',
            'Refactoring',
            'Domain-Driven Design',
            'Design Patterns',
            'The Pragmatic Programmer',
            'Code Complete',
            'Working Effectively with Legacy Code',
            'Patterns of Enterprise Application Architecture',
            'Head First Design Patterns',
            'Continuous Delivery',
            'Release It!',
            'The Mythical Man-Month',
            'Peopleware',
            'Structure and Interpretation of Computer Programs',
            'Introduction to Algorithms',
            'The C Programming Language',
            'Effective Java',
            'Programming Pearls',
            'The Art of Computer Programming',
            'Building Microservices',
            'Site Reliability Engineering',
            'Designing Data-Intensive Applications',
            'Effective C#',
            'Clean Architecture',
            'Soft Skills'
        ]::text[] AS titles,
        ARRAY[
            'Robert C. Martin',
            'Martin Fowler',
            'Eric Evans',
            'Erich Gamma et al.',
            'Andrew Hunt',
            'Steve McConnell',
            'Michael Feathers',
            'Martin Fowler',
            'Eric Freeman',
            'Jez Humble',
            'Michael T. Nygard',
            'Frederick P. Brooks Jr.',
            'Tom DeMarco',
            'Harold Abelson',
            'Thomas H. Cormen',
            'Brian W. Kernighan',
            'Joshua Bloch',
            'Jon Bentley',
            'Donald Knuth',
            'Sam Newman',
            'Betsy Beyer',
            'Martin Kleppmann',
            'Bill Wagner',
            'Robert C. Martin',
            'John Sonmez'
        ]::text[] AS authors
)
INSERT INTO books (id, title, author, published_year, isbn)
SELECT
    (
        substr(md5('real-book-' || i::text), 1, 8) || '-' ||
        substr(md5('real-book-' || i::text), 9, 4) || '-' ||
        substr(md5('real-book-' || i::text), 13, 4) || '-' ||
        substr(md5('real-book-' || i::text), 17, 4) || '-' ||
        substr(md5('real-book-' || i::text), 21, 12)
    )::uuid,
    CASE
        WHEN c.cycle = 0 THEN b.base_title
        ELSE b.base_title || ' (Edition ' || (c.cycle + 1)::text || ')'
    END,
    b.base_author,
    1950 + (i % 74),
    '978-1-' || lpad((100000000 + i)::text, 9, '0')
FROM generate_series(1, 5000) AS s(i)
CROSS JOIN data d
CROSS JOIN LATERAL (
    SELECT
        1 + ((i - 1) % array_length(d.titles, 1)) AS idx,
        ((i - 1) / array_length(d.titles, 1)) AS cycle
) AS c
CROSS JOIN LATERAL (
    SELECT
        d.titles[c.idx] AS base_title,
        d.authors[c.idx] AS base_author
) AS b;
");

            migrationBuilder.Sql(
                @"
WITH data AS (
    SELECT
        ARRAY[
            'Nature',
            'Science',
            'Cell',
            'The Lancet',
            'New England Journal of Medicine',
            'JAMA',
            'IEEE Transactions on Software Engineering',
            'ACM Computing Surveys',
            'Communications of the ACM',
            'IEEE Software',
            'Artificial Intelligence',
            'Journal of Machine Learning Research',
            'The Astrophysical Journal',
            'Physical Review Letters',
            'PLOS ONE'
        ]::text[] AS journal_titles,
        ARRAY[
            'Springer Nature',
            'AAAS',
            'Cell Press',
            'Elsevier',
            'Massachusetts Medical Society',
            'American Medical Association',
            'IEEE',
            'ACM',
            'ACM',
            'IEEE',
            'Elsevier',
            'JMLR.org',
            'IOP Publishing',
            'APS',
            'Public Library of Science'
        ]::text[] AS publishers
)
INSERT INTO journals (id, title, issue_number, publication_year, publisher)
SELECT
    (
        substr(md5('real-journal-' || i::text), 1, 8) || '-' ||
        substr(md5('real-journal-' || i::text), 9, 4) || '-' ||
        substr(md5('real-journal-' || i::text), 13, 4) || '-' ||
        substr(md5('real-journal-' || i::text), 17, 4) || '-' ||
        substr(md5('real-journal-' || i::text), 21, 12)
    )::uuid,
    j.base_title || ' Vol ' || (1 + ((i - 1) / array_length(d.journal_titles, 1)))::text,
    ((i - 1) % 24) + 1,
    2000 + (i % 27),
    j.base_publisher
FROM generate_series(1, 1000) AS s(i)
CROSS JOIN data d
CROSS JOIN LATERAL (
    SELECT
        1 + ((i - 1) % array_length(d.journal_titles, 1)) AS idx
) AS c
CROSS JOIN LATERAL (
    SELECT
        d.journal_titles[c.idx] AS base_title,
        d.publishers[c.idx] AS base_publisher
) AS j;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
DELETE FROM clients
WHERE id IN (
    SELECT
        (
            substr(md5('real-client-' || i::text), 1, 8) || '-' ||
            substr(md5('real-client-' || i::text), 9, 4) || '-' ||
            substr(md5('real-client-' || i::text), 13, 4) || '-' ||
            substr(md5('real-client-' || i::text), 17, 4) || '-' ||
            substr(md5('real-client-' || i::text), 21, 12)
        )::uuid
    FROM generate_series(1, 100) AS s(i)
);
");

            migrationBuilder.Sql(
                @"
DELETE FROM books
WHERE id IN (
    SELECT
        (
            substr(md5('real-book-' || i::text), 1, 8) || '-' ||
            substr(md5('real-book-' || i::text), 9, 4) || '-' ||
            substr(md5('real-book-' || i::text), 13, 4) || '-' ||
            substr(md5('real-book-' || i::text), 17, 4) || '-' ||
            substr(md5('real-book-' || i::text), 21, 12)
        )::uuid
    FROM generate_series(1, 5000) AS s(i)
);
");

            migrationBuilder.Sql(
                @"
DELETE FROM journals
WHERE id IN (
    SELECT
        (
            substr(md5('real-journal-' || i::text), 1, 8) || '-' ||
            substr(md5('real-journal-' || i::text), 9, 4) || '-' ||
            substr(md5('real-journal-' || i::text), 13, 4) || '-' ||
            substr(md5('real-journal-' || i::text), 17, 4) || '-' ||
            substr(md5('real-journal-' || i::text), 21, 12)
        )::uuid
    FROM generate_series(1, 1000) AS s(i)
);
");

            migrationBuilder.Sql(
                @"
INSERT INTO clients (id, first_name, last_name, email, registered_at_utc)
SELECT
    (
        substr(md5('client-' || i::text), 1, 8) || '-' ||
        substr(md5('client-' || i::text), 9, 4) || '-' ||
        substr(md5('client-' || i::text), 13, 4) || '-' ||
        substr(md5('client-' || i::text), 17, 4) || '-' ||
        substr(md5('client-' || i::text), 21, 12)
    )::uuid,
    'SeedFirst' || i::text,
    'SeedLast' || i::text,
    'seed.client.' || i::text || '@example.com',
    now() - (i::text || ' days')::interval
FROM generate_series(1, 100) AS s(i);
");

            migrationBuilder.Sql(
                @"
INSERT INTO books (id, title, author, published_year, isbn)
SELECT
    (
        substr(md5('book-' || i::text), 1, 8) || '-' ||
        substr(md5('book-' || i::text), 9, 4) || '-' ||
        substr(md5('book-' || i::text), 13, 4) || '-' ||
        substr(md5('book-' || i::text), 17, 4) || '-' ||
        substr(md5('book-' || i::text), 21, 12)
    )::uuid,
    'Seed Book ' || i::text,
    'Seed Author ' || (((i - 1) % 50) + 1)::text,
    1980 + (i % 45),
    'SEED-ISBN-' || lpad(i::text, 5, '0')
FROM generate_series(1, 5000) AS s(i);
");

            migrationBuilder.Sql(
                @"
INSERT INTO journals (id, title, issue_number, publication_year, publisher)
SELECT
    (
        substr(md5('journal-' || i::text), 1, 8) || '-' ||
        substr(md5('journal-' || i::text), 9, 4) || '-' ||
        substr(md5('journal-' || i::text), 13, 4) || '-' ||
        substr(md5('journal-' || i::text), 17, 4) || '-' ||
        substr(md5('journal-' || i::text), 21, 12)
    )::uuid,
    'Seed Journal ' || i::text,
    ((i - 1) % 24) + 1,
    2000 + (i % 27),
    'Seed Publisher ' || (((i - 1) % 20) + 1)::text
FROM generate_series(1, 1000) AS s(i);
");
        }
    }
}
