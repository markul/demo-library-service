Perfect! Now I have all the information needed. Let me create the implementation plan based on the Jira issue and Confluence documentation.

## Implementation Plan for DEMO-19: Add API for Subscription Payment in Library Service

Based on the Jira issue DEMO-19 and linked Confluence pages, here is the comprehensive implementation plan:

### 1. **Current State Analysis**
- Jira Issue: DEMO-19 - "Добавить api для оплаты подписки в Library Service" (Add API for subscription payment in Library Service)
- Labels: db-write, ebooks, integration, payments, saga
- Status: Backlog
- Priority: Medium

### 2. **Key Requirements from Confluence**

#### A. Main Process Page: "Процесс оформления подписки" (Subscription Checkout Process)
- **Input**: `subscriptionTypeId`, `clientId`, `idempotencyKey`
- **Output**: `SubscriptionCheckoutResult` with `SubscriptionId` and `PaymentStatus`
- **Key Steps**:
  1. Validate client and subscription type
  2. Calculate final price (with discount logic)
  3. Create subscription record
  4. Create payment record
  5. Call PaymentService externally
  6. Update records based on payment result
  7. Return checkout result

#### B. Price Calculation Page: "Алгоритм расчёта цены подписки" (Subscription Price Calculation Algorithm)
- **Base Price**: from `subscription_types.price`
- **Discount Logic**: 
  - If client has >5 subscriptions → 5% discount
  - Otherwise, no discount
- **Formula**: `finalPrice = Round(basePrice * (1 - discountPercent/100), 2)`

#### C. API Endpoint: "/api/subscriptions/checkout"
- **Method**: POST
- **Route**: `/api/subscriptions/checkout`
- **Request Body**: `clientId`, `subscriptionTypeId`, `idempotencyKey`
- **Response**: `SubscriptionCheckoutResult`
- **Status Codes**: 201 Created, 200 OK (idempotent), 404 Not Found, 402 Payment Required

### 3. **Implementation Tasks**

#### A. **Domain Layer Changes**
1. **Create DTOs**:
   - `SubscriptionCheckoutRequest` (clientId, subscriptionTypeId, idempotencyKey)
   - `SubscriptionCheckoutResult` (subscriptionId, paymentStatus)
   - `PriceCalculationResult` (basePrice, finalPrice, discountPercent)
   - `PriceCalculationInput` (subscriptionTypeId, clientId)

#### B. **Application Layer Changes**
1. **Commands**:
   - Create `CheckoutSubscriptionCommand` handler
   - Implement price calculation logic
   - Implement subscription creation with validation
   - Implement payment creation with idempotency support

2. **Services** (if needed):
   - Create `IPriceCalculator` interface
   - Implement `PriceCalculator` service

#### C. **Infrastructure Layer Changes**
1. **EF Configuration**:
   - Update `PaymentConfiguration` if needed
   - Ensure proper relationships

2. **Database Migrations**:
   - No schema changes needed (current model supports this)

#### D. **API Layer Changes**
1. **Controller**:
   - Add `Checkout` action in `SubscriptionsController`
   - Handle idempotency via `idempotencyKey`
   - Return appropriate status codes

2. **HTTP client definitions**:
   - Add endpoint to `LibraryService.Api.http` file

#### E. **External Service Integration**
1. **PaymentService Client**:
   - Reference existing PaymentService.Client package
   - Configure HTTP client for PaymentService
   - Implement retry logic for payment calls

#### F. **Documentation Updates**
1. **API Documentation**:
   - Add `CheckoutSubscription.md` to ApiDocs/Subscriptions/
   - Add UML diagram for checkout flow
   - Add request/response examples

2. **Update Existing Docs**:
   - Link new checkout endpoint from existing subscription documentation

#### G. **Testing**
1. **Unit Tests**:
   - Test price calculation logic
   - Test discount thresholds (5 subscriptions)
   - Test idempotency handling

2. **Integration Tests**:
   - Test full checkout flow
   - Test payment service integration
   - Test error scenarios (client not found, invalid subscription type)

### 4. **Technical Notes**

- **Saga Pattern**: The labels suggest implementing a saga pattern for consistency across services
- **Idempotency**: Use `idempotencyKey` to prevent duplicate processing
- **Database Transactions**: Use EF Core transactions for consistency
- **Payment States**: Map `Processing` → `Paid/Failed` based on PaymentService response
- **Discount Logic**: Count existing subscriptions for client before applying discount

### 5. **Estimated Complexity**
- **Domain/Application Logic**: Medium
- **External Integration**: Medium (depends on PaymentService stability)
- **Testing**: Medium (need comprehensive test coverage)

### 6. **Dependencies**
- PaymentService must be running and accessible
- Database migrations may be needed if schema changes (currently no changes needed)
- Client should have valid subscription type and client IDs

Would you like me to start implementing any specific part of this plan?


### CASE RESULTS
- Failed.
- Comments: Had explicitly tell agent to get linked pages to the jira issue. Agent managed to get jira issue, created implementation plan which mentions idempotency and transactions. On the implemtation the agent failed. overall 4-5 attepmts. Screenshots of the failed attempts is attached.