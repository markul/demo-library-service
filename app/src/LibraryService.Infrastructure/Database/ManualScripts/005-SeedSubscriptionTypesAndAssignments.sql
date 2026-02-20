START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213113045_SeedSubscriptionTypesAndAssignments') THEN

    INSERT INTO subscription_types (id, name, period, price)
    VALUES
        ('11111111-1111-1111-1111-111111111106'::uuid, '6 month', 6, 59.99),
        ('11111111-1111-1111-1111-111111111112'::uuid, '12 month', 12, 99.99),
        ('11111111-1111-1111-1111-111111111300'::uuid, '3 years', 36, 249.99),
        ('11111111-1111-1111-1111-111111111500'::uuid, '5 years', 60, 349.99);

    WITH client_choice AS (
        SELECT
            c.id AS client_id,
            gen_random_uuid() AS subscription_id,
            (ARRAY[
                '11111111-1111-1111-1111-111111111106'::uuid,
                '11111111-1111-1111-1111-111111111112'::uuid,
                '11111111-1111-1111-1111-111111111300'::uuid,
                '11111111-1111-1111-1111-111111111500'::uuid
            ])[1 + floor(random() * 4)::int] AS subscription_type_id
        FROM clients c
    ),
    inserted_subscriptions AS (
        INSERT INTO subscriptions (id, name, subscription_type_id, is_active, start_date_utc)
        SELECT
            cc.subscription_id,
            CONCAT('Auto subscription ', LEFT(cc.client_id::text, 8)),
            cc.subscription_type_id,
            TRUE,
            NOW() AT TIME ZONE 'UTC'
        FROM client_choice cc
    )
    INSERT INTO client_subscriptions (client_id, subscription_id)
    SELECT cc.client_id, cc.subscription_id
    FROM client_choice cc;

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213113045_SeedSubscriptionTypesAndAssignments') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260213113045_SeedSubscriptionTypesAndAssignments', '8.0.11');
    END IF;
END $EF$;
COMMIT;

