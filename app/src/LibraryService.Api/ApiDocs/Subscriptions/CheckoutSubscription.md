# CheckoutSubscription

## Purpose

Initiates a subscription checkout process for a client. This endpoint handles the complete flow of subscribing a client to a subscription type, calculating the final price, creating the subscription record, and processing the payment through an external payment service.

## Endpoint

`POST /api/subscriptions/checkout`

## Parameters

### Request Body

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `subscriptionTypeId` | `Guid` | Yes | Identifier of the subscription type (source of base price) |
| `clientId` | `Guid` | Yes | Identifier of the client (used for subscription count calculation) |
| `idempotencyKey` | `string` | Yes | Unique key for idempotent payment processing |

### Example Request

```json
{
  "subscriptionTypeId": "11b6338c-272e-1b16-d98c-05396dc10dc6",
  "clientId": "22c7449d-383f-2c27-e09d-16407dc21ed7",
  "idempotencyKey": "order-12345-checkout"
}
```

## Responses

### Success (200 OK)

Returns the checkout result with subscription and payment status.

```json
{
  "subscriptionId": "33d8550e-494a-3d38-f18e-27518dc32fe8",
  "paymentStatus": "Paid",
  "amount": 1500.00
}
```

### Error Responses

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid request data or validation failed |
| 404 Not Found | Client or subscription type not found |
| 409 Conflict | Idempotency key already used for a different request |
| 502 Bad Gateway | Payment service unavailable or returned an error |

## Process Flow

1. **Validate Client and Subscription Type**
   - Verify client exists in `public.clients` table
   - Verify subscription type exists in `public.subscription_types` table

2. **Calculate Final Price**
   - Calculate subscription price based on client's existing subscription count
   - Apply discount rules according to pricing algorithm

3. **Create Subscription Record**
   - Insert new subscription into `public.subscriptions`
   - Set `is_active = false` (pending payment)
   - Set `start_date_utc = current date`
   - Set `subscription_type_id` from request

4. **Create Payment Record**
   - Insert new payment into `public.payments`
   - Set `status = Processing`
   - Set `unique_id = idempotencyKey`
   - Set `subscription_id` to new subscription's id

5. **Process Payment via PaymentService**
   - Send payment request to external payment service
   - On success:
     - Set `subscription.is_active = true`
     - Set `payment.status = Paid`
     - Set `payment.external_id` from payment service response
   - On decline:
     - Set `payment.status = Failed`
   - On error:
     - Keep `payment.status = Processing` for retry
     - Keep `subscription.is_active = false`

6. **Return Result**
   - Return subscription ID and payment status

## Diagram

See [Diagrams/CheckoutSubscription/Algorithm.svg](Diagrams/CheckoutSubscription/Algorithm.svg) for the visual process flow.

## Related

- [Algorithm for calculating subscription price](#) - Link to pricing algorithm documentation
- [Payment Service Integration](#) - Link to payment service documentation
