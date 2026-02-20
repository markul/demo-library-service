START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    DROP TABLE books;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    DROP TABLE clients;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    DROP TABLE journals;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213064741_InitialLibrarySchema') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260213064741_InitialLibrarySchema';
    END IF;
END $EF$;
COMMIT;

