START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260226095147_AddClientAddresses') THEN
    DROP TABLE client_addresses;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260226095147_AddClientAddresses') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260226095147_AddClientAddresses';
    END IF;
END $EF$;
COMMIT;

