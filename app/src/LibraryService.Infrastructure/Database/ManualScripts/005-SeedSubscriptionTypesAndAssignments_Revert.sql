START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213113045_SeedSubscriptionTypesAndAssignments') THEN

    DELETE FROM payments
    WHERE subscription_id IN (
        SELECT s.id
        FROM subscriptions s
        WHERE s.name LIKE 'Auto subscription %'
          AND s.subscription_type_id IN (
              '11111111-1111-1111-1111-111111111106'::uuid,
              '11111111-1111-1111-1111-111111111112'::uuid,
              '11111111-1111-1111-1111-111111111300'::uuid,
              '11111111-1111-1111-1111-111111111500'::uuid
          )
    );

    DELETE FROM client_subscriptions
    WHERE subscription_id IN (
        SELECT s.id
        FROM subscriptions s
        WHERE s.name LIKE 'Auto subscription %'
          AND s.subscription_type_id IN (
              '11111111-1111-1111-1111-111111111106'::uuid,
              '11111111-1111-1111-1111-111111111112'::uuid,
              '11111111-1111-1111-1111-111111111300'::uuid,
              '11111111-1111-1111-1111-111111111500'::uuid
          )
    );

    DELETE FROM subscriptions
    WHERE name LIKE 'Auto subscription %'
      AND subscription_type_id IN (
          '11111111-1111-1111-1111-111111111106'::uuid,
          '11111111-1111-1111-1111-111111111112'::uuid,
          '11111111-1111-1111-1111-111111111300'::uuid,
          '11111111-1111-1111-1111-111111111500'::uuid
      );

    DELETE FROM subscription_types
    WHERE id IN (
        '11111111-1111-1111-1111-111111111106'::uuid,
        '11111111-1111-1111-1111-111111111112'::uuid,
        '11111111-1111-1111-1111-111111111300'::uuid,
        '11111111-1111-1111-1111-111111111500'::uuid
    );

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260213113045_SeedSubscriptionTypesAndAssignments') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260213113045_SeedSubscriptionTypesAndAssignments';
    END IF;
END $EF$;
COMMIT;

