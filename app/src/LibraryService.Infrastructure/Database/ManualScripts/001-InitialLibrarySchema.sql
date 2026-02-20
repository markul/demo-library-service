CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    CREATE TABLE books (
        id uuid NOT NULL,
        title character varying(200) NOT NULL,
        author character varying(200) NOT NULL,
        published_year integer NOT NULL,
        isbn character varying(64) NOT NULL,
        CONSTRAINT "PK_books" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    CREATE TABLE clients (
        id uuid NOT NULL,
        first_name character varying(100) NOT NULL,
        last_name character varying(100) NOT NULL,
        email character varying(255) NOT NULL,
        registered_at_utc timestamp with time zone NOT NULL,
        CONSTRAINT "PK_clients" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    CREATE TABLE journals (
        id uuid NOT NULL,
        title character varying(200) NOT NULL,
        issue_number integer NOT NULL,
        publication_year integer NOT NULL,
        publisher character varying(200) NOT NULL,
        CONSTRAINT "PK_journals" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    CREATE UNIQUE INDEX "IX_clients_email" ON clients (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260213064741_InitialLibrarySchema', '8.0.11');
    END IF;
END $EF$;
COMMIT;

