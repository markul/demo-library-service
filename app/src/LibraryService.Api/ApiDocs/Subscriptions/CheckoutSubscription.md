# Checkout Subscription

## Purpose

Creates a new subscription with an associated payment for a client. This endpoint handles the complete checkout flow including validation, subscription creation, and payment initialization.

## Endpoint

`POST /api/subscriptions/checkout`

## Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| subscriptionTypeId | Guid | Yes | Identifier of the subscription type to purchase |
| clientId | Guid | Yes | Identifier of the client making the purchase |
| idempotencyKey | string | Yes | Unique key for idempotent operations (prevents duplicate charges) |

## Request

```json
{
  "subscriptionTypeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clientId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "idempotencyKey": "checkout-550e8400-e29b-41d4-a716-446655440000"
}
```

## Response

### Success (200 OK)

```json
{
  "subscriptionId": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
  "paymentStatus": "Processing"
}
```

### Payment Status Values

| Status | Description |
|--------|-------------|
| New | Payment newly created |
| Processing | Payment is being processed |
| Paid | Payment completed successfully |
| Cancelled | Payment was cancelled |
| Failed | Payment processing failed |

### Error Responses

| Status Code | Description |
|-------------|-------------|
| 404 | Client or subscription type not found |
| 400 | Invalid request parameters |

## Behavior

1. **Idempotency Check**: If a payment with the same `idempotencyKey` exists, returns the existing subscription and payment status
2. **Validation**: Verifies that both client and subscription type exist
3. **Subscription Creation**: Creates a new subscription with `isActive = false`
4. **Payment Creation**: Creates a payment with `status = Processing`
5. **Return Result**: Returns the subscription ID and payment status

## Notes

- The subscription is created in an inactive state
- Payment processing happens asynchronously
- Use the `idempotencyKey` to safely retry failed requests
- The payment status will be updated via webhook or polling mechanism
