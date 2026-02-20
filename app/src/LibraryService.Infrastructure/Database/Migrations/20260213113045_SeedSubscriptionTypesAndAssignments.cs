using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryService.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedSubscriptionTypesAndAssignments : Migration
    {
        private const string SixMonthTypeId = "11111111-1111-1111-1111-111111111106";
        private const string TwelveMonthTypeId = "11111111-1111-1111-1111-111111111112";
        private const string ThreeYearsTypeId = "11111111-1111-1111-1111-111111111300";
        private const string FiveYearsTypeId = "11111111-1111-1111-1111-111111111500";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
INSERT INTO subscription_types (id, name, period, price)
VALUES
    ('{SixMonthTypeId}'::uuid, '6 month', 6, 59.99),
    ('{TwelveMonthTypeId}'::uuid, '12 month', 12, 99.99),
    ('{ThreeYearsTypeId}'::uuid, '3 years', 36, 249.99),
    ('{FiveYearsTypeId}'::uuid, '5 years', 60, 349.99);

WITH client_choice AS (
    SELECT
        c.id AS client_id,
        gen_random_uuid() AS subscription_id,
        (ARRAY[
            '{SixMonthTypeId}'::uuid,
            '{TwelveMonthTypeId}'::uuid,
            '{ThreeYearsTypeId}'::uuid,
            '{FiveYearsTypeId}'::uuid
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
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
DELETE FROM payments
WHERE subscription_id IN (
    SELECT s.id
    FROM subscriptions s
    WHERE s.name LIKE 'Auto subscription %'
      AND s.subscription_type_id IN (
          '{SixMonthTypeId}'::uuid,
          '{TwelveMonthTypeId}'::uuid,
          '{ThreeYearsTypeId}'::uuid,
          '{FiveYearsTypeId}'::uuid
      )
);

DELETE FROM client_subscriptions
WHERE subscription_id IN (
    SELECT s.id
    FROM subscriptions s
    WHERE s.name LIKE 'Auto subscription %'
      AND s.subscription_type_id IN (
          '{SixMonthTypeId}'::uuid,
          '{TwelveMonthTypeId}'::uuid,
          '{ThreeYearsTypeId}'::uuid,
          '{FiveYearsTypeId}'::uuid
      )
);

DELETE FROM subscriptions
WHERE name LIKE 'Auto subscription %'
  AND subscription_type_id IN (
      '{SixMonthTypeId}'::uuid,
      '{TwelveMonthTypeId}'::uuid,
      '{ThreeYearsTypeId}'::uuid,
      '{FiveYearsTypeId}'::uuid
  );

DELETE FROM subscription_types
WHERE id IN (
    '{SixMonthTypeId}'::uuid,
    '{TwelveMonthTypeId}'::uuid,
    '{ThreeYearsTypeId}'::uuid,
    '{FiveYearsTypeId}'::uuid
);
");
        }
    }
}
