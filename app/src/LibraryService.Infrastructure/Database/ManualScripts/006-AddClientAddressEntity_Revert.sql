START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260303134358_AddClientAddressEntity') THEN
    DROP TABLE client_addresses;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260303134358_AddClientAddressEntity') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260303134358_AddClientAddressEntity';
    END IF;
END $EF$;
COMMIT;

