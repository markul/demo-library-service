## DEMO-19: Subscription Checkout API Implementation Plan

### Summary
- Implement `POST /api/subscriptions/checkout` from Jira `DEMO-19` using linked Confluence specs (`Процесс оформления подписки`, `Алгоритм расчёта цены подписки`, `/api/subscriptions/checkout`).
- Add idempotent checkout flow that creates subscription + payment, calls external payment service, and returns `subscriptionId` + `paymentStatus`.
- Keep current DB schema (no migration required), using existing `subscriptions`, `client_subscriptions`, and `payments` tables.

### Implementation Changes
- **Public API contract**
1. Add endpoint `POST /api/subscriptions/checkout` in subscriptions controller.
2. Request body: `clientId` (Guid), `subscriptionTypeId` (Guid), `idempotencyKey` (string, required, max 128).
3. Response body: `{ subscriptionId: Guid, paymentStatus: string }` where `paymentStatus` is string (`Paid|Failed|Processing`).
4. Status code mapping:
   - `201 Created`: first-time request, payment accepted.
   - `200 OK`: idempotent repeat with existing successful checkout.
   - `202 Accepted`: payment is still processing (including technical failure path).
   - `402 Payment Required`: payment rejected (including idempotent repeats of rejected checkout).
   - `404 Not Found`: client or subscription type not found.
   - `409 Conflict`: same `idempotencyKey` reused with different `clientId`/`subscriptionTypeId`.

- **Application layer**
1. Add checkout command + result model (with explicit outcome/status code mapping so controller stays thin).
2. Add application abstractions for:
   - checkout persistence/query operations (idempotency lookup + create/update flow),
   - external payment call adapter (to avoid Application -> Infrastructure dependency).
3. Checkout handler algorithm:
   - Lookup by `idempotencyKey`.
   - If found, validate same parameters; return existing result or `409`.
   - Validate client and subscription type.
   - Calculate price: `basePrice = subscription_types.price`; if client has `> 5` subscriptions apply `5%` discount; `finalPrice = Round(basePrice * 0.95, 2)`.
   - Create subscription (`is_active=false`, `start_date_utc=UtcNow`, `subscription_type_id` set, `name="Checkout subscription"`), link to client, create payment (`status=Processing`, `unique_id=idempotencyKey`, `amount=finalPrice`).
   - Call external payment service.
   - Map external result: `Accepted -> Paid + activate subscription + set external_id`; `Rejected -> Failed`; technical error/timeout -> keep `Processing` and inactive.
   - Persist and return mapped HTTP outcome/result body.

- **Infrastructure layer**
1. Implement checkout repository with EF Core queries/updates and transactional local write steps.
2. Implement payment adapter using `PaymentService.Client`:
   - `ClientId = clientId.ToString("D")`
   - `Amount = finalPrice`
   - `Currency` from config (`PaymentService:Currency`, default `USD`)
   - Deterministic description (e.g., checkout + subscription id/type info).
3. Register new abstractions in DI.
4. Update `appsettings.json` and `appsettings.Development.json` with `PaymentService:Currency`.

- **Docs and examples**
1. Update `LibraryService.Api.http` with checkout sample request.
2. Add API docs in `ApiDocs/Subscriptions` for checkout:
   - method markdown,
   - input/output examples,
   - `.puml` and `.svg` diagram folder.

### Test Plan
1. Unit tests for checkout handler:
   - returns `404` for missing client/type,
   - applies discount only when subscriptions count `> 5`,
   - creates Processing payment before external call,
   - maps Accepted/Rejected/exception to `Paid/Failed/Processing`,
   - idempotent repeat returns existing result without duplicate inserts,
   - mismatched payload with same key returns `409`.
2. Integration tests for new endpoint:
   - `201` on first successful checkout,
   - `200` on idempotent repeat (same key/same payload),
   - `402` on rejected payment and retry of rejected checkout,
   - `202` on technical failure path,
   - `404` for unknown references,
   - `409` for key reuse with different payload.
3. Validation/build checks:
   - `dotnet build app/LibraryService.sln`
   - `dotnet test app/LibraryService.sln`

### Assumptions and Locked Defaults
- `idempotencyKey` comparison is exact string match and bounded to 128 chars.
- Same key + different business payload is treated as conflict (`409`).
- Failed idempotent retries return `402` (not `200`).
- Technical payment-call failures return `202` with persisted `Processing`.
- Required subscription name for checkout is fixed constant: `"Checkout subscription"`.
- Payment currency comes from configuration with fallback `USD`.
