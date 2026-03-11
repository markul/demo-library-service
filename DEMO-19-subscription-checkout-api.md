# Implementation Plan: DEMO-19 - Subscription Checkout API

## Jira Issue

**Key:** DEMO-19  
**Summary:** Добавить api для оплаты подписки в Library Service  
**Type:** История (Story)  
**Status:** Backlog  
**Priority:** Medium  
**Assignee:** Marat  

## Objective

Add the ability to pay for a subscription. The checkout process should handle client validation, price calculation, subscription creation, and payment processing through an external payment service.

## Reference Documentation

- Confluence: [Процесс оформления подписки](http://localhost:8090/spaces/DEMO/pages/5373977)

---

## Architecture Overview

### Current State

The project already has:
- [`Subscription`](app/src/LibraryService.Domain/Entities/Subscription.cs) entity with `IsActive`, `StartDateUtc`, `SubscriptionTypeId` properties
- [`Payment`](app/src/LibraryService.Domain/Entities/Payment.cs) entity with `Status`, `UniqueId`, `ExternalId`, `Amount` properties
- [`PaymentStatus`](app/src/LibraryService.Domain/Entities/PaymentStatus.cs) enum with `New`, `Processing`, `Paid`, `Cancelled`, `Failed` values
- [`SubscriptionType`](app/src/LibraryService.Domain/Entities/SubscriptionType.cs) entity with `Price` property
- [`Client`](app/src/LibraryService.Domain/Entities/Client.cs) entity
- Repository interfaces and implementations for all entities
- Existing subscription endpoints in [`SubscriptionsController`](app/src/LibraryService.Api/Controllers/SubscriptionsController.cs)

### Target State

Add a new `POST /api/subscriptions/checkout` endpoint that:
1. Validates client and subscription type existence
2. Calculates the final subscription price
3. Creates a subscription (inactive) and payment (processing)
4. Calls external payment service
5. Updates subscription and payment status based on payment result
6. Returns checkout result

---

## Implementation Steps

### Phase 1: Domain Layer

#### 1.1 Add Payment Service Abstraction

Create `IPaymentService` interface in Application layer for external payment integration.

**File:** `app/src/LibraryService.Application/Abstractions/Services/IPaymentService.cs`

```csharp
namespace LibraryService.Application.Abstractions.Services;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(
        Guid clientId, 
        Guid subscriptionId, 
        decimal amount, 
        string idempotencyKey, 
        CancellationToken cancellationToken);
}

public record PaymentResult(
    bool IsSuccess, 
    string? ExternalId = null, 
    string? ErrorMessage = null);
```

#### 1.2 Add Price Calculation Service

Create `ISubscriptionPricingService` for calculating subscription prices.

**File:** `app/src/LibraryService.Application/Abstractions/Services/ISubscriptionPricingService.cs`

```csharp
namespace LibraryService.Application.Abstractions.Services;

public interface ISubscriptionPricingService
{
    Task<decimal> CalculatePriceAsync(
        Guid subscriptionTypeId, 
        Guid clientId, 
        CancellationToken cancellationToken);
}
```

---

### Phase 2: Application Layer

#### 2.1 Create Checkout Command

Create the checkout command and handler.

**File:** `app/src/LibraryService.Application/Subscriptions/Commands/CheckoutSubscriptionCommand.cs`

```csharp
public record CheckoutSubscriptionCommand(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey) : IRequest<CheckoutResult?>;

public record CheckoutResult(
    Guid SubscriptionId,
    PaymentStatus PaymentStatus,
    decimal Amount);
```

The handler will:
1. Validate client exists via `IClientRepository`
2. Validate subscription type exists via `ISubscriptionTypeRepository`
3. Calculate price via `ISubscriptionPricingService`
4. Create subscription (inactive) via `ISubscriptionRepository`
5. Create payment (processing) via `IPaymentRepository`
6. Call `IPaymentService.ProcessPaymentAsync`
7. Update subscription and payment status based on result
8. Return `CheckoutResult`

#### 2.2 Create Request DTO

**File:** `app/src/LibraryService.Application/Subscriptions/SubscriptionDto.cs` (extend existing)

```csharp
public record CheckoutSubscriptionRequest(
    Guid SubscriptionTypeId,
    Guid ClientId,
    string IdempotencyKey);
```

---

### Phase 3: Infrastructure Layer

#### 3.1 Implement Payment Service

Create a mock/placeholder implementation for the payment service.

**File:** `app/src/LibraryService.Infrastructure/Services/PaymentService.cs`

```csharp
public class PaymentService : IPaymentService
{
    // Implementation for external payment gateway integration
    // For now, simulate successful payment
}
```

#### 3.2 Implement Pricing Service

**File:** `app/src/LibraryService.Infrastructure/Services/SubscriptionPricingService.cs`

```csharp
public class SubscriptionPricingService : ISubscriptionPricingService
{
    // Calculate price based on:
    // - Base price from SubscriptionType
    // - Client's existing subscription count (for discounts)
}
```

#### 3.3 Extend Subscription Repository

Add method to count client subscriptions for pricing.

**File:** `app/src/LibraryService.Application/Abstractions/Repositories/ISubscriptionRepository.cs`

```csharp
Task<int> CountClientSubscriptionsAsync(Guid clientId, CancellationToken cancellationToken);
```

---

### Phase 4: API Layer

#### 4.1 Add Checkout Endpoint

**File:** `app/src/LibraryService.Api/Controllers/SubscriptionsController.cs`

Add new endpoint:

```csharp
[HttpPost("checkout")]
public async Task<ActionResult<CheckoutResult>> Checkout(
    CheckoutSubscriptionRequest request, 
    CancellationToken cancellationToken)
{
    var command = new CheckoutSubscriptionCommand(
        request.SubscriptionTypeId,
        request.ClientId,
        request.IdempotencyKey);
    var result = await _mediator.Send(command, cancellationToken);
    return result is null ? NotFound("Client or subscription type not found") : Ok(result);
}
```

---

### Phase 5: Dependency Injection

#### 5.1 Register Services

**File:** `app/src/LibraryService.Infrastructure/ServiceCollectionExtensions.cs`

```csharp
services.AddScoped<IPaymentService, PaymentService>();
services.AddScoped<ISubscriptionPricingService, SubscriptionPricingService>();
```

---

### Phase 6: API Documentation

#### 6.1 Create API Docs

Already created: [`CheckoutSubscription.md`](app/src/LibraryService.Api/ApiDocs/Subscriptions/CheckoutSubscription.md)

#### 6.2 Create Diagram

**File:** `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/Algorithm.puml`

#### 6.3 Create Examples

- `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Input.md`
- `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Output.md`

---

### Phase 7: HTTP Client Configuration

#### 7.1 Update .http File

**File:** `app/src/LibraryService.Api/LibraryService.Api.http`

Add checkout endpoint example.

---

### Phase 8: Testing

#### 8.1 Unit Tests

Create test project if not exists:
- `app/test/LibraryService.Application.Tests/Subscriptions/Commands/CheckoutSubscriptionCommandHandlerTests.cs`
- `app/test/LibraryService.Infrastructure.Tests/Services/PaymentServiceTests.cs`
- `app/test/LibraryService.Infrastructure.Tests/Services/SubscriptionPricingServiceTests.cs`

#### 8.2 Integration Tests

- `app/test/LibraryService.Api.Tests/Controllers/SubscriptionsControllerTests.cs` - Add checkout endpoint tests

---

## File Changes Summary

### New Files

| File | Purpose |
|------|---------|
| `app/src/LibraryService.Application/Abstractions/Services/IPaymentService.cs` | Payment service interface |
| `app/src/LibraryService.Application/Abstractions/Services/ISubscriptionPricingService.cs` | Pricing service interface |
| `app/src/LibraryService.Application/Subscriptions/Commands/CheckoutSubscriptionCommand.cs` | Checkout command and handler |
| `app/src/LibraryService.Infrastructure/Services/PaymentService.cs` | Payment service implementation |
| `app/src/LibraryService.Infrastructure/Services/SubscriptionPricingService.cs` | Pricing service implementation |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/Algorithm.puml` | Process diagram |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Diagrams/CheckoutSubscription/Algorithm.svg` | Generated diagram |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Input.md` | Request example |
| `app/src/LibraryService.Api/ApiDocs/Subscriptions/Examples/CheckoutSubscription/Output.md` | Response example |

### Modified Files

| File | Changes |
|------|---------|
| `app/src/LibraryService.Application/Abstractions/Repositories/ISubscriptionRepository.cs` | Add `CountClientSubscriptionsAsync` method |
| `app/src/LibraryService.Infrastructure/Repositories/SubscriptionRepository.cs` | Implement `CountClientSubscriptionsAsync` |
| `app/src/LibraryService.Application/Subscriptions/SubscriptionDto.cs` | Add `CheckoutSubscriptionRequest` and `CheckoutResult` |
| `app/src/LibraryService.Api/Controllers/SubscriptionsController.cs` | Add checkout endpoint |
| `app/src/LibraryService.Infrastructure/ServiceCollectionExtensions.cs` | Register new services |
| `app/src/LibraryService.Api/LibraryService.Api.http` | Add checkout endpoint example |

---

## Database Considerations

No database schema changes required. The existing tables support the checkout flow:
- `public.clients` - Client validation
- `public.subscription_types` - Subscription type validation and base price
- `public.subscriptions` - New subscription records
- `public.payments` - Payment records with idempotency key

---

## Error Handling

| Scenario | HTTP Status | Response |
|----------|-------------|----------|
| Client not found | 404 | `{"error": "Client not found"}` |
| Subscription type not found | 404 | `{"error": "Subscription type not found"}` |
| Duplicate idempotency key | 409 | `{"error": "Payment already processed"}` |
| Payment service unavailable | 502 | `{"error": "Payment service unavailable"}` |
| Payment declined | 200 | `CheckoutResult` with `PaymentStatus.Failed` |

---

## Implementation Order

1. ✅ Create API documentation (CheckoutSubscription.md)
2. ⬜ Create service interfaces (IPaymentService, ISubscriptionPricingService)
3. ⬜ Extend ISubscriptionRepository with count method
4. ⬜ Implement repository method
5. ⬜ Implement services (PaymentService, SubscriptionPricingService)
6. ⬜ Create CheckoutSubscriptionCommand and handler
7. ⬜ Add DTOs (CheckoutSubscriptionRequest, CheckoutResult)
8. ⬜ Add controller endpoint
9. ⬜ Register services in DI
10. ⬜ Create diagram and examples
11. ⬜ Update .http file
12. ⬜ Write unit tests
13. ⬜ Write integration tests
14. ⬜ Build and verify

---

## Acceptance Criteria

- [ ] `POST /api/subscriptions/checkout` endpoint is available
- [ ] Returns 404 when client doesn't exist
- [ ] Returns 404 when subscription type doesn't exist
- [ ] Creates subscription with `is_active = false` initially
- [ ] Creates payment with `status = Processing` initially
- [ ] Calls payment service with idempotency key
- [ ] Updates subscription `is_active = true` on successful payment
- [ ] Updates payment `status = Paid` on successful payment
- [ ] Updates payment `status = Failed` on declined payment
- [ ] Returns correct `CheckoutResult` with subscription ID and payment status
- [ ] API documentation is complete
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Solution builds successfully
