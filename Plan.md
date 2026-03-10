# Implementation Plan for DEMO-19: Subscription Checkout API

## Issue Summary

**Issue Key:** DEMO-19  
**Type:** Story  
**Status:** Backlog  
**Priority:** Medium  
**Labels:** db-write, ebooks, integration, payments, saga

**Description:** Implementation of API for managing subscriptions in Library Service

**Reference:** [Confluence Page](http://localhost:8090/spaces/DEMO/pages/5373977)

---

## Requirements

### Input Parameters
- `subscriptionTypeId` - Subscription type ID
- `clientId` - Client ID
- `idempotencyKey` - Unique key for idempotent payment processing

### Output
- `SubscriptionCheckoutResult` containing:
  - `SubscriptionId`
  - `PaymentStatus`

### Business Logic Flow

1. **Validate client exists** in `public.clients` table
2. **Validate subscription type exists** in `public.subscription_types` table
3. **Create new subscription** in `public.subscriptions` table:
   - `is_active = false`
   - `start_date_utc = current timestamp`
   - `subscription_type_id = subscriptionTypeId`
4. **Create new payment** in `public.payments` table:
   - `Status = Processing`
   - `unique_id = idempotencyKey`
   - `subscription_id = NewSubscription.id`
5. **Call PaymentService** (external service):
   - If payment succeeds:
     - `NewSubscription.is_active = true`
     - `NewPayment.status = Paid`
     - `NewPayment.external_id = payment service id`
   - If payment fails:
     - `NewPayment.status = Failed`
   - If payment processing:
     - `NewPayment.Status = Processing`
     - `NewSubscription.is_active = false`
6. **Return result**

---

## Implementation Plan

### 1. New Application Layer Components

#### 1.1 DTOs (LibraryService.Application)

**File:** `app/src/LibraryService.Application/Subscriptions/SubscriptionCheckoutDto.cs` (NEW)

```csharp
namespace LibraryService.Application.Subscriptions;

public record SubscriptionCheckoutResult(
    Guid SubscriptionId,
    PaymentStatus PaymentStatus);
```

#### 1.2 Command (LibraryService.Application)

**File:** `app/src/LibraryService.Application/Subscriptions/Commands/CheckoutSubscriptionCommand.cs` (NEW)

```csharp
public record CheckoutSubscriptionCommand(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey) : IRequest<SubscriptionCheckoutResult?>;
```

#### 1.3 Command Handler (LibraryService.Application)

**File:** `app/src/LibraryService.Application/Subscriptions/Commands/CheckoutSubscriptionCommandHandler.cs` (NEW)

The handler will:
- Validate client exists
- Validate subscription type exists
- Create subscription with `is_active = false`
- Create payment with `Status = Processing`
- Call external PaymentService
- Update subscription and payment based on payment result
- Return `SubscriptionCheckoutResult`

### 2. New API Layer Components

#### 2.1 Controller Endpoint (LibraryService.Api)

**File:** `app/src/LibraryService.Api/Controllers/SubscriptionsController.cs` (MODIFY)

Add new endpoint:

```csharp
[HttpPost("checkout")]
public async Task<ActionResult<SubscriptionCheckoutResult>> Checkout(
    CheckoutSubscriptionRequest request, 
    CancellationToken cancellationToken)
{
    var command = new CheckoutSubscriptionCommand(
        request.SubscriptionTypeId,
        request.ClientId,
        request.IdempotencyKey);
    var result = await _mediator.Send(command, cancellationToken);
    if (result is null)
    {
        return NotFound("Subscription type or client was not found.");
    }
    return Ok(result);
}
```

#### 2.2 Request DTO (LibraryService.Application)

**File:** `app/src/LibraryService.Application/Subscriptions/SubscriptionDto.cs` (MODIFY)

Add:

```csharp
public record CheckoutSubscriptionRequest(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey);
```

### 3. API Documentation

#### 3.1 API Docs (LibraryService.Api)

**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/CheckoutSubscription.md` (NEW)

Include:
- Purpose
- Parameters
- Examples
- Responses

#### 3.2 Diagrams

**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/` (NEW)

- `Algorithm.puml` - Algorithm diagram
- `Algorithm.svg` - Algorithm diagram (SVG)
- `CheckoutSubscription.puml` - Sequence diagram
- `CheckoutSubscription.svg` - Sequence diagram (SVG)

#### 3.3 Examples

**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/` (NEW)

- `Input.md` - Example request
- `Output.md` - Example response

### 4. Database Changes

**No database changes required** - existing tables (`clients`, `subscription_types`, `subscriptions`, `payments`) are sufficient.

### 5. Testing

#### 5.1 Unit Tests

**File:** `app/test/LibraryService.Application.Tests/Subscriptions/Commands/CheckoutSubscriptionCommandHandlerTests.cs` (NEW)

Test cases:
- Valid subscription type and client - should create subscription and payment
- Invalid subscription type - should return null
- Invalid client - should return null
- Payment service success - should activate subscription
- Payment service failure - should mark payment as failed
- Payment service processing - should keep subscription inactive

#### 5.2 Integration Tests

**File:** `app/test/LibraryService.Api.Tests/Controllers/SubscriptionsControllerTests.cs` (MODIFY)

Test cases:
- POST /api/subscriptions/checkout with valid data
- POST /api/subscriptions/checkout with invalid subscription type
- POST /api/subscriptions/checkout with invalid client

### 6. Implementation Steps

1. **Create DTOs**
   - Add `SubscriptionCheckoutResult` to `SubscriptionCheckoutDto.cs`

2. **Create Command**
   - Add `CheckoutSubscriptionCommand` and handler

3. **Update API Layer**
   - Add `CheckoutSubscriptionRequest` to `SubscriptionDto.cs`
   - Add `Checkout` endpoint to `SubscriptionsController.cs`

4. **Update API Documentation**
   - Create `CheckoutSubscription.md`
   - Create diagrams
   - Create examples

5. **Add Tests**
   - Create unit tests for handler
   - Create integration tests for controller

6. **Build and Verify**
   - Run `dotnet build app/LibraryService.sln`
   - Run `dotnet test app/LibraryService.sln`

---

## Files to Create

| File | Purpose |
|------|---------|
| `app/src/LibraryService.Application/Subscriptions/SubscriptionCheckoutDto.cs` | Result DTO |
| `app/src/LibraryService.Application/Subscriptions/Commands/CheckoutSubscriptionCommand.cs` | Command definition |
| `app/src/LibraryService.Application/Subscriptions/Commands/CheckoutSubscriptionCommandHandler.cs` | Command handler |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/CheckoutSubscription.md` | API documentation |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/Algorithm.puml` | Algorithm diagram (source) |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/Algorithm.svg` | Algorithm diagram (rendered) |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/CheckoutSubscription.puml` | Sequence diagram (source) |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/CheckoutSubscription.svg` | Sequence diagram (rendered) |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Input.md` | Example request |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Output.md` | Example response |
| `app/test/LibraryService.Application.Tests/Subscriptions/Commands/CheckoutSubscriptionCommandHandlerTests.cs` | Unit tests |
| `app/test/LibraryService.Api.Tests/Controllers/SubscriptionsControllerTests.cs` | Integration tests (modify) |

---

## Files to Modify

| File | Changes |
|------|---------|
| `app/src/LibraryService.Application/Subscriptions/SubscriptionDto.cs` | Add `CheckoutSubscriptionRequest` |
| `app/src/LibraryService.Api/Controllers/SubscriptionsController.cs` | Add `Checkout` endpoint |
| `app/src/LibraryService.Api/LibraryService.Api.http` | Add checkout endpoint test |

---

## Notes

- The implementation follows the existing patterns in the project (CQRS with MediatR)
- PaymentService integration will require external service configuration
- The saga pattern is implied by the multi-step process with external service call
- Idempotency is handled via `unique_id` in payments table
- All database operations use existing tables and relationships
