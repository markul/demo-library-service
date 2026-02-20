# UpdatePayment

## Purpose
Updates an existing payment.

## Endpoint
PUT /api/payments/{id}

## Parameters
id (GUID): payment identifier. Body: uniqueId, amount, subscriptionId, clientId, externalId, status.

## Examples
- Input: Examples/UpdatePayment/Input.md
- Output: Examples/UpdatePayment/Output.md

## Responses
- Success: 204 No Content
- Failure: 404 Not Found

## Algorithm
![Algorithm](./Diagrams/UpdatePayment/Algorithm.svg)
