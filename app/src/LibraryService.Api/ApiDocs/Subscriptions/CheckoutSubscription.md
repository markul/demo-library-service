# CheckoutSubscription

## Purpose
Creates a subscription checkout and initiates payment in the external payment service.

## Endpoint
POST /api/subscriptions/checkout

## Parameters
Body: clientId, subscriptionTypeId, idempotencyKey.

## Examples
- Input: Examples/CheckoutSubscription/Input.md
- Output: Examples/CheckoutSubscription/Output.md

## Responses
- Success: 201 Created (new paid checkout)
- Success: 200 OK (idempotent retry of paid checkout)
- Success: 202 Accepted (payment is processing)
- Failure: 402 Payment Required (payment rejected)
- Failure: 404 Not Found
- Failure: 409 Conflict

## Algorithm
![Algorithm](./Diagrams/CheckoutSubscription/Algorithm.svg)
