START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData') THEN

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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData') THEN

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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData') THEN

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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData') THEN

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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData') THEN

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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData') THEN

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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260213071851_SeedRealisticLibraryData';
    END IF;
END $EF$;
COMMIT;

