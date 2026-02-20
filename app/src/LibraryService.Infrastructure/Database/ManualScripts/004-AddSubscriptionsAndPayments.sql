START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE TABLE subscription_types (
        id uuid NOT NULL,
        name character varying(100) NOT NULL,
        period integer NOT NULL,
        price numeric(18,2) NOT NULL,
        CONSTRAINT "PK_subscription_types" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE TABLE subscriptions (
        id uuid NOT NULL,
        name character varying(200) NOT NULL,
        subscription_type_id uuid NOT NULL,
        is_active boolean NOT NULL,
        start_date_utc timestamp with time zone NOT NULL,
        CONSTRAINT "PK_subscriptions" PRIMARY KEY (id),
        CONSTRAINT "FK_subscriptions_subscription_types_subscription_type_id" FOREIGN KEY (subscription_type_id) REFERENCES subscription_types (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE TABLE client_subscriptions (
        client_id uuid NOT NULL,
        subscription_id uuid NOT NULL,
        CONSTRAINT "PK_client_subscriptions" PRIMARY KEY (client_id, subscription_id),
        CONSTRAINT "FK_client_subscriptions_clients_client_id" FOREIGN KEY (client_id) REFERENCES clients (id) ON DELETE CASCADE,
        CONSTRAINT "FK_client_subscriptions_subscriptions_subscription_id" FOREIGN KEY (subscription_id) REFERENCES subscriptions (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE TABLE payments (
        id uuid NOT NULL,
        unique_id character varying(128) NOT NULL,
        amount numeric(18,2) NOT NULL,
        subscription_id uuid NOT NULL,
        client_id uuid NOT NULL,
        external_id character varying(128),
        status character varying(32) NOT NULL,
        CONSTRAINT "PK_payments" PRIMARY KEY (id),
        CONSTRAINT "FK_payments_clients_client_id" FOREIGN KEY (client_id) REFERENCES clients (id) ON DELETE RESTRICT,
        CONSTRAINT "FK_payments_subscriptions_subscription_id" FOREIGN KEY (subscription_id) REFERENCES subscriptions (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE INDEX "IX_client_subscriptions_subscription_id" ON client_subscriptions (subscription_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE INDEX "IX_payments_client_id" ON payments (client_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE INDEX "IX_payments_external_id" ON payments (external_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE INDEX "IX_payments_subscription_id" ON payments (subscription_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE UNIQUE INDEX "IX_payments_unique_id" ON payments (unique_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE INDEX "IX_subscription_types_name" ON subscription_types (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    CREATE INDEX "IX_subscriptions_subscription_type_id" ON subscriptions (subscription_type_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260213104622_AddSubscriptionsAndPayments', '8.0.11');
    END IF;
END $EF$;
COMMIT;

