START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260305104207_AddClientAddress') THEN
    CREATE TABLE client_addresses (
        id uuid NOT NULL,
        client_id uuid NOT NULL,
        city character varying(100) NOT NULL,
        country character varying(100) NOT NULL,
        address character varying(255) NOT NULL,
        postal_code character varying(20) NOT NULL,
        CONSTRAINT "PK_client_addresses" PRIMARY KEY (id),
        CONSTRAINT "FK_client_addresses_clients_client_id" FOREIGN KEY (client_id) REFERENCES clients (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260305104207_AddClientAddress') THEN
    CREATE INDEX "IX_client_addresses_client_id" ON client_addresses (client_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260305104207_AddClientAddress') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260305104207_AddClientAddress', '8.0.11');
    END IF;
END $EF$;
COMMIT;

