START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    DROP TABLE client_subscriptions;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    DROP TABLE payments;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    DROP TABLE subscriptions;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    DROP TABLE subscription_types;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260213104622_AddSubscriptionsAndPayments';
    END IF;
END $EF$;
COMMIT;

