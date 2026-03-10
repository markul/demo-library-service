# Implementation Plan: DEMO-19 - Subscription Checkout API

## Jira Issue Summary

| Field | Value |
|-------|-------|
| **Key** | DEMO-19 |
| **Summary** | Create API for subscription checkout in Library Service |
| **Type** | Story |
| **Status** | Backlog |
| **Priority** | Medium |
| **Assignee** | Marat |
| **Reporter** | Marat |
| **Created** | 2026-03-05 |
| **Confluence Reference** | [Subscription Checkout Process](http://localhost:8090/spaces/DEMO/pages/5373977) |

## Overview

Implement a new API endpoint for subscription checkout that handles the complete flow of creating a subscription with payment processing. The endpoint should validate input, create subscription and payment records, and integrate with an external payment service.

## Business Requirements

### Input Parameters
- `subscriptionTypeId` - Identifier of the subscription type (must exist in database)
- `clientId` - Identifier of the client (must exist in database)
- `idempotencyKey` - Unique key for idempotent operations

### Output (SubscriptionCheckoutResult)
- `SubscriptionId` - Created subscription identifier
- `PaymentStatus` - Status of the payment

## Implementation Steps

### Step 1: Create Domain Models and DTOs

#### 1.1 Create Checkout Request/Response DTOs
**File:** `app/src/LibraryService.Application/Subscriptions/SubscriptionCheckoutDto.cs`

```csharp
namespace LibraryService.Application.Subscriptions;

public record SubscriptionCheckoutRequest(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey);

public record SubscriptionCheckoutResult(
    Guid SubscriptionId,
    PaymentStatus PaymentStatus);
```

### Step 2: Add Repository Methods

#### 2.1 Extend ISubscriptionRepository Interface
**File:** `app/src/LibraryService.Application/Abstractions/Repositories/ISubscriptionRepository.cs`

Add methods:
- `Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken)` - if not already available
- `Task<Subscription?> GetByIdWithPaymentAsync(Guid id, CancellationToken cancellationToken)` - load subscription with payments

#### 2.2 Extend IPaymentRepository Interface
**File:** `app/src/LibraryService.Application/Abstractions/Repositories/IPaymentRepository.cs`

Add methods:
- `Task<Payment?> GetByUniqueIdAsync(string uniqueId, CancellationToken cancellationToken)` - for idempotency check

#### 2.3 Implement Repository Methods
**File:** `app/src/LibraryService.Infrastructure/Repositories/SubscriptionRepository.cs`
**File:** `app/src/LibraryService.Infrastructure/Repositories/PaymentRepository.cs`

### Step 3: Create Payment Service Abstraction

#### 3.1 Define IPaymentService Interface
**File:** `app/src/LibraryService.Application/Abstractions/Services/IPaymentService.cs`

```csharp
namespace LibraryService.Application.Abstractions.Services;

public record PaymentServiceRequest(
    Guid PaymentId,
    decimal Amount,
    string IdempotencyKey);

public record PaymentServiceResult(
    bool Success,
    string? ExternalId,
    string? ErrorMessage);

public interface IPaymentService
{
    Task<PaymentServiceResult> ProcessPaymentAsync(PaymentServiceRequest request, CancellationToken cancellationToken);
}
```

#### 3.2 Implement Mock Payment Service (for development)
**File:** `app/src/LibraryService.Infrastructure/Services/MockPaymentService.cs`

### Step 4: Create Checkout Command

#### 4.1 Create SubscriptionCheckoutCommand
**File:** `app/src/LibraryService.Application/Subscriptions/Commands/SubscriptionCheckoutCommand.cs`

**Business Logic Flow:**

1. **Idempotency Check**
   - Check if payment with `idempotencyKey` already exists
   - If exists, return existing subscription and payment status

2. **Validation**
   - Verify client exists in `public.clients`
   - Verify subscription type exists in `public.subscription_types`

3. **Create Subscription**
   - `NewSubscription.is_active = false`
   - `NewSubscription.start_date_utc = current date`
   - `NewSubscription.subscription_type_id = subscriptionTypeId`
   - `NewSubscription.client_ids = [clientId]`

4. **Create Payment**
   - `NewPayment.Status = Processing`
   - `NewPayment.unique_id = idempotencyKey`
   - `NewPayment.subscription_id = NewSubscription.id`
   - `NewPayment.amount = SubscriptionType.price`

5. **Call Payment Service**
   - If payment successful:
     - `NewSubscription.is_active = true`
     - `NewPayment.status = Paid`
     - `NewPayment.external_id = id from payment service`
   - If payment failed:
     - `NewPayment.status = Failed`
   - If payment pending/timeout:
     - `NewPayment.status = Processing`
     - `NewSubscription.is_active = false`

6. **Return Result**
   - Return `SubscriptionCheckoutResult` with subscription ID and payment status

### Step 5: Add API Endpoint

#### 5.1 Add Checkout Endpoint to Controller
**File:** `app/src/LibraryService.Api/Controllers/SubscriptionsController.cs`

```csharp
[HttpPost("checkout")]
public async Task<ActionResult<SubscriptionCheckoutResult>> Checkout(
    SubscriptionCheckoutRequest request,
    CancellationToken cancellationToken)
{
    var command = new SubscriptionCheckoutCommand(
        request.SubscriptionTypeId,
        request.ClientId,
        request.IdempotencyKey);
    var result = await _mediator.Send(command, cancellationToken);
    return Ok(result);
}
```

### Step 6: Register Services

#### 6.1 Update DI Registration
**File:** `app/src/LibraryService.Api/Program.cs` or `app/src/LibraryService.Application/ServiceCollectionExtensions.cs`

```csharp
services.AddScoped<IPaymentService, MockPaymentService>();
```

### Step 7: Add API Documentation

#### 7.1 Create API Documentation
**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/CheckoutSubscription.md`

Include:
- Purpose
- Parameters
- Request/Response examples
- Error scenarios

#### 7.2 Create Diagrams
**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/Algorithm.puml`

#### 7.3 Create Examples
**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Input.md`
**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Output.md`

### Step 8: Update HTTP Test File

**File:** `app/src/LibraryService.Api/LibraryService.Api.http`

Add checkout endpoint test request.

### Step 9: Add Unit Tests

**File:** `app/test/LibraryService.Application.Tests/Subscriptions/Commands/SubscriptionCheckoutCommandTests.cs`

Test cases:
- `Handle_ValidRequest_ReturnsSubscriptionCheckoutResult`
- `Handle_InvalidClientId_ReturnsNotFound`
- `Handle_InvalidSubscriptionTypeId_ReturnsNotFound`
- `Handle_DuplicateIdempotencyKey_ReturnsExistingResult`
- `Handle_PaymentSuccess_SetsSubscriptionActive`
- `Handle_PaymentFailed_SetsPaymentFailed`
- `Handle_PaymentPending_KeepsPaymentProcessing`

## Technical Considerations

### Transaction Management
- Use `IUnitOfWork` pattern or EF Core transaction to ensure atomicity
- All database operations (subscription, payment creation, updates) should be in a single transaction

### Idempotency
- Use `idempotencyKey` (stored in `Payment.unique_id`) to prevent duplicate processing
- Return existing result if duplicate request detected

### Error Handling
- Return appropriate error responses for:
  - Client not found (404)
  - Subscription type not found (404)
  - Payment processing errors (502 or appropriate status)

### Security
- Validate that the client belongs to the authenticated user (if authentication is implemented)
- Consider rate limiting for checkout endpoint

## Dependencies

| Component | Status | Notes |
|-----------|--------|-------|
| Subscription entity | ✅ Exists | May need minor updates |
| Payment entity | ✅ Exists | Has all required fields |
| PaymentStatus enum | ✅ Exists | Has Processing, Paid, Failed |
| ISubscriptionRepository | ✅ Exists | Needs extension |
| IPaymentRepository | ✅ Exists | Needs extension |
| IPaymentService | ❌ New | Needs to be created |

## Estimated Effort

| Task | Hours |
|------|-------|
| DTOs and interfaces | 1 |
| Repository extensions | 2 |
| Payment service implementation | 2 |
| Command handler | 4 |
| Controller endpoint | 1 |
| API documentation | 2 |
| Unit tests | 3 |
| Integration testing | 2 |
| **Total** | **17 hours** |

## Acceptance Criteria

- [ ] API endpoint `POST /api/subscriptions/checkout` is available
- [ ] Valid request creates subscription with payment
- [ ] Idempotency is handled correctly
- [ ] Invalid client returns 404
- [ ] Invalid subscription type returns 404
- [ ] Payment success activates subscription
- [ ] Payment failure marks payment as failed
- [ ] API documentation is complete
- [ ] Unit tests pass
- [ ] Build succeeds

## Risks and Mitigations

| Risk | Mitigation |
|------|------------|
| Payment service integration complexity | Start with mock implementation, define clear interface |
| Transaction deadlocks | Keep transactions short, use appropriate isolation level |
| Idempotency key collisions | Use GUID format for idempotency keys |

## Related Documents

- [Confluence: Subscription Checkout Process](http://localhost:8090/spaces/DEMO/pages/5373977)
- [Jira: DEMO-19](http://localhost:8080/browse/DEMO-19)
